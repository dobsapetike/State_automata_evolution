using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using BenchmarkDepot.Classes.Core.EAlgotihms;

namespace BenchmarkDepot.Classes.Core
{

    /// <summary>
    /// Class reprezentation of a finite state, deterministic transducer
    /// </summary>
    public class Transducer : ICloneable, IComparable
    {

        #region Private fields

        /// <summary>
        /// List of states in the transducer
        /// </summary>
        private List<TransducerState> _states;

        /// <summary>
        /// Currently active state
        /// </summary>
        private TransducerState _currentState;

        /// <summary>
        /// Startig state
        /// </summary>
        private TransducerState _startState;

        #endregion 

        #region Properties

        /// <summary>
        /// Output generated so far
        /// </summary>
        public string Translation { get; private set; }

        /// <summary>
        /// A read-only getter for the collection of states
        /// </summary>
        public ReadOnlyCollection<TransducerState> States
        {
            get { return _states.AsReadOnly(); }
        }

        /// <summary>
        /// Auto-property for the evaluation info
        /// </summary>
        public EvaluationInfo EvaluationInfo
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        public Transducer()
        {
            _states = new List<TransducerState>();
            EvaluationInfo = new EvaluationInfo();
        }

        #endregion 

        #region Cloning

        public object Clone()
        {
            var other = (Transducer)this.MemberwiseClone();
            other._states = _states.Select(state => (TransducerState)state.Clone()).ToList();
            if (_currentState != null)
            {
                other._currentState = other._states.Where(state => state.ID == _currentState.ID).First();
            }
            if (_startState != null)
            {
                other._startState = other._states.Where(state => state.ID == _startState.ID).First();
            }
            other.EvaluationInfo = new EvaluationInfo();
            return other;
        }

        #endregion

        #region Comparer

        /// <summary>
        /// Comparison for sorting by fitness value
        /// In case of fitness equality compares by age
        /// </summary>
        public int CompareTo(object obj)
        {
            var other = (Transducer) obj;
            if (other.EvaluationInfo.Fitness != EvaluationInfo.Fitness)
            {
                return this.EvaluationInfo.Fitness.CompareTo(other.EvaluationInfo.Fitness);
            }
            return other.EvaluationInfo.Age.CompareTo(this.EvaluationInfo.Age);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Method for adding new states
        /// </summary>
        /// <returns>If a state with the same Id already exists returns it, otherwise returns
        /// the new state</returns>
        public TransducerState AddState(TransducerState state)
        {
            var exists = _states.Where(s => s.ID == state.ID).FirstOrDefault();
            if (exists != null) return exists;

            _states.Add(state);
            if (_states.Count == 1)
            {
                _startState = state;
                _currentState = state;
            }
            return state;
        }

        /// <summary>
        /// Performs state shifting from the currently active state based on the triggering event
        /// </summary>
        /// <param name="trigger">Action/event that invokes the shifting</param>
        /// <returns>whether the state shifting could be realized</returns>
        public bool ShiftState(TransitionTrigger trigger, double param = -1d)
        {
            var tuple = _currentState.ShiftState(trigger, param);
            if (tuple == null) return false;

            _currentState = _states.Where(item => item.ID == tuple.Item1).First();
            Translation += tuple.Item2;
            return true;
        }

        /// <summary>
        /// Inserts a transition between two states. 
        /// If any of the states doesn't exist in the transducer, it will be added.
        /// If a transition with the same action exists between these states, it will be overwritten
        /// </summary>
        public void AddTransition(TransducerState from, TransducerState to, TransitionTrigger trigger, TransducerTransition transition)
        {
            // id control with 'AddState' - so two states with same Id but different reference won't be added
            var a = AddState(from);
            var b = AddState(to);
            
            transition.StateFrom = a.ID;
            transition.StateTo = b.ID;
            transition.TransitionTrigger = trigger;
            from.AddTransition(trigger.TransitionEvent, transition, b.ID);
        }

        /// <summary>
        /// Sets back the transducer to the starting state
        /// </summary>
        public void Reset()
        {
            Translation = "";
            _currentState = _startState;
        }

        /// <summary>
        /// Removes a state with given index
        /// </summary>
        public void RemoveStateAt(int index)
        {
            if (index < 1 || index >= _states.Count)
            {
                throw new IndexOutOfRangeException();
            }
            if (_currentState == _states[index])
            {
                _currentState = null;
            }
            _states.RemoveAt(index);
        }

        /// <summary>
        /// String representation of the transducer for debugging purposes
        /// </summary>
        public override string ToString()
        {
            string result = "";
            foreach (var state in _states)
            {
                if (state == _startState)
                {
                    result += "*";
                }
                result += state.ToString();
            }
            return result;
        }

        #endregion

    }

}
