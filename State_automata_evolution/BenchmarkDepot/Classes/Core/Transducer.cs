using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core
{

    /// <summary>
    /// Class reprezentation of a finite state, deterministic transducer
    /// </summary>
    public class Transducer : ICloneable
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

        #endregion 

        #region Properties

        /// <summary>
        /// Output generated so far
        /// </summary>
        public string Translation { get; private set; }

        /// <summary>
        /// A read-only getter for the state collection
        /// </summary>
        public ReadOnlyCollection<TransducerState> States
        {
            get { return _states.AsReadOnly(); }
        }

        #endregion

        #region Constructor

        public Transducer()
        {
            _states = new List<TransducerState>();
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
            return other;
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
            if (_states.Count == 1) _currentState = state;
            return state;
        }

        /// <summary>
        /// Performs state shifting from the currently active state based on the triggering event
        /// </summary>
        /// <param name="trigger">Action/event that invokes the shifting</param>
        public void ShiftState(string trigger)
        {
            var tuple = _currentState.ShiftState(trigger);
            if (tuple == null) return;
            _currentState = _states.Where(item => item.ID == tuple.Item1).First();
            Translation += tuple.Item2;
        }

        /// <summary>
        /// Inserts a transition between two states. 
        /// If any of the states doesn't exist in the transducer, it will be added.
        /// If a transition with the same action exists between these states, it will be overwritten
        /// </summary>
        public void AddTransition(TransducerState from, TransducerState to, string action, TransducerTransition transition)
        {
            // Id control with 'AddState' - so two states with same Id but different reference won't be added
            to = AddState(to);
            from = AddState(from);

            transition.StateFrom = from.ID;
            transition.StateTo = to.ID;
            transition.TransitionEvent = action;
            from.AddTransition(action, transition, to.ID);
        }

        public override string ToString()
        {
            string result = "";
            foreach (var state in _states)
            {
                result += state.ToString();
            }
            return result;
        }

        #endregion

    }

}
