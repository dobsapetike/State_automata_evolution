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
        private Dictionary<TransitionTrigger, Tuple<TransducerTransition, int>> _transitions;

        #region Constructor

        public TransducerState(int id)
        {
            ID = id;
            _transitions = new Dictionary<TransitionTrigger, Tuple<TransducerTransition, int>>();
        }

        #endregion

        #region Cloning

        public object Clone()
        {
            var other = (TransducerState)this.MemberwiseClone();
            other._transitions = new Dictionary<TransitionTrigger, Tuple<TransducerTransition, int>>();
            foreach (var transition in _transitions)
            {
                other._transitions.Add(transition.Key, new Tuple<TransducerTransition, int>
                    ((TransducerTransition)transition.Value.Item1.Clone(), transition.Value.Item2));
            }
            return other;
        }

        #endregion

        public void AddTransition(TransitionTrigger trigger, TransducerTransition transition, int destinationId)
        {
           _transitions[trigger] = new Tuple<TransducerTransition, int>(transition, destinationId);
        }

        public bool RemoveTransition(TransitionTrigger trigger)
        {
            return _transitions.Remove(trigger);
        }

        public bool RemoveTransition(int Id)
        {
            var action = GetTransitionEvent(Id);
            if (action == null) return false;

            return RemoveTransition(action.Value);
        }

        /// <summary>
        /// Clears the transition list of this state
        /// </summary>
        public void ResetTransitions()
        {
            _transitions.Clear();
        }

        public TransducerTransition GetTransition(int stateID)
        {
            var transition = _transitions.Where(item => item.Value.Item2 == stateID).FirstOrDefault();
            return transition.Equals(default(KeyValuePair<TransitionTrigger, Tuple<TransducerTransition, int>>))
                ? null : transition.Value.Item1;
        }

        private TransitionTrigger? GetTransitionEvent(int stateID)
        {
            var transition = _transitions.Where(item => item.Value.Item2 == stateID).FirstOrDefault();
            return transition.Equals(default(KeyValuePair<TransitionTrigger, Tuple<TransducerTransition, int>>))
                ? (TransitionTrigger?)null : transition.Key;
        }

        public List<TransducerTransition> GetListOfTransitions()
        {
            var result = new List<TransducerTransition>();
            result = _transitions.Select(item => (TransducerTransition)item.Value.Item1.Clone()).ToList();
            return result;
        }

        /// <summary>
        /// Gets the id of the state where the transition - triggered by the given event - leads
        /// </summary>
        /// <returns>State id if such trigger exists, else -1</returns>
        public int GetDestinationIdByTrigger(TransitionTrigger trigger)
        {
            if (!_transitions.ContainsKey(trigger)) return -1;
            return _transitions[trigger].Item2;
        }

        /// <summary>
        /// This method will be called when the automaton switches state
        /// </summary>
        /// <param name="action">The action that invokes the switching</param>
        /// <returns>Tuple of the id of state activated by the switching and the generated output. 
        /// Returns null if shifting couldn't be realised</returns>
        public Tuple<int, string> ShiftState(TransitionTrigger trigger)
        {
            if (!_transitions.ContainsKey(trigger)) return null;

            var shift = _transitions[trigger];
            var s = shift.Item1.DoAction();
            return new Tuple<int, string>(shift.Item2, s);
        }

        public override string ToString()
        {
            string result = "";
            foreach (var state in _transitions)
            {
                result += String.Format("{0,-15} -> {1,-15} -> ", ID, state.Key.TransitionEvent.SetStringToLenght(15));
                result += String.Format("{0,-15} // ", state.Value.Item2);
                result += String.Format("{0,-15}({1})\n", state.Value.Item1.Translation.SetStringToLenght(15), state.Value.Item1.InnovationNumber);
            }
            return result;
        }

    }

}
