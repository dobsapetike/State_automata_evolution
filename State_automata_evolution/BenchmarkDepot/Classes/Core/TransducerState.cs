using System;
using System.Linq;
using System.Collections.Generic;
using BenchmarkDepot.Classes.Extensions;

namespace BenchmarkDepot.Classes.Core
{

    /// <summary>
    /// Class representation of a transition state
    /// </summary>
    public class TransducerState : ICloneable
    {

        /// <summary>
        /// Id for ensuring that the same transitions won't get different innovation numbers
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Associative data structure for storing all the transitions from this state
        /// Key: the action which invokes state shifting as a string
        /// Value: the id of the state activated by executig this action and the transition that leads there
        /// </summary>
        private Dictionary<string, Tuple<TransducerTransition, int>> _transitions;

        #region Constructor

        public TransducerState(int id)
        {
            ID = id;
            _transitions = new Dictionary<string, Tuple<TransducerTransition, int>>();
        }

        #endregion

        #region Cloning

        public object Clone()
        {
            var other = (TransducerState)this.MemberwiseClone();
            other._transitions = new Dictionary<string, Tuple<TransducerTransition, int>>();
            foreach (KeyValuePair<string, Tuple<TransducerTransition, int>> transition in _transitions)
            {
                other._transitions.Add(transition.Key, new Tuple<TransducerTransition, int>
                    ((TransducerTransition)transition.Value.Item1.Clone(), transition.Value.Item2));
            }
            return other;
        }

        #endregion

        public void AddTransition(string action, TransducerTransition transition, int destinationId)
        {
            _transitions[action] = new Tuple<TransducerTransition, int>(transition, destinationId);
        }

        public bool RemoveTransition(string action)
        {
            return _transitions.Remove(action);
        }

        public bool RemoveTransition(int Id)
        {
            var action = GetTransitionEvent(Id);
            if (action == String.Empty) return false;

            return RemoveTransition(action);
        }

        public TransducerTransition GetTransition(int stateID)
        {
            var transition = _transitions.Where(item => item.Value.Item2 == stateID).FirstOrDefault();
            return transition.Equals(default(KeyValuePair<string, Tuple<TransducerTransition, int>>))
                ? null : transition.Value.Item1;
        }

        public string GetTransitionEvent(int stateID)
        {
            var transition = _transitions.Where(item => item.Value.Item2 == stateID).FirstOrDefault();
            return transition.Equals(default(KeyValuePair<string, Tuple<TransducerTransition, int>>))
                ? String.Empty : transition.Key;
        }

        /// <summary>
        /// This method will be called when the automaton switches state
        /// </summary>
        /// <param name="action">The action that invokes the switching</param>
        /// <returns>Tuple of the id of state activated by the switching and the generated output</returns>
        public Tuple<int, string> ShiftState(string action)
        {
            if (!_transitions.ContainsKey(action)) return null;

            var shift = _transitions[action];
            var s = shift.Item1.DoAction();
            return new Tuple<int, string>(shift.Item2, s);
        }

        public override string ToString()
        {
            string result = "";
            foreach (var state in _transitions)
            {
                result += String.Format("{0,-15} -> {1,-15} -> ", ID, state.Key.SetStringToLenght(15));
                result += String.Format("{0,-15} // ", state.Value.Item2);
                result += String.Format("{0,-15}\n", state.Value.Item1.Translation.SetStringToLenght(15));
            }
            return result;
        }

    }

}
