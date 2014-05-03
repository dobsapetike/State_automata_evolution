using System;
using System.Linq;
using System.Collections.Generic;
using BenchmarkDepot.Classes.Misc;

namespace BenchmarkDepot.Classes.Core
{

    /// <summary>
    /// Class representation of a transition state
    /// </summary>
    public class TransducerState
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

        public TransducerState Clone()
        {
            var state = new TransducerState(ID);
            foreach (var transition in _transitions)
            {
                state._transitions.Add(transition.Key, new Tuple<TransducerTransition, int>
                    (transition.Value.Item1.Clone(), transition.Value.Item2));
            }
            return state;
        }

        #endregion

        /// <summary>
        /// Inserts a new transition starting from this state.
        /// If a transition with the given trigger already exists, will be replaced
        /// </summary>
        public void AddTransition(string trigger, TransducerTransition transition, int destinationId)
        {
           _transitions[trigger] = new Tuple<TransducerTransition, int>(transition, destinationId);
        }

        /// <summary>
        /// Removes transition with the given trigger
        /// </summary>
        /// <returns>true if transition was found and removed - otherwise false</returns>
        public bool RemoveTransition(string trigger)
        {
            return _transitions.Remove(trigger);
        }

        /// <summary>
        /// Removes the first found transition that leads to a state with the given id
        /// </summary>
        /// <returns>true if trigger was found and removed - otherwise false</returns>
        public bool RemoveTransition(int Id)
        {
            var action = GetTransitionEvent(Id);
            if (action == String.Empty) return false;

            return RemoveTransition(action);
        }

        /// <summary>
        /// Clears the transition list of this state
        /// </summary>
        public void ResetTransitions()
        {
            _transitions.Clear();
        }

        /// <summary>
        /// Gets the first found transition thats leads to a state with the given id number
        /// </summary>
        /// <returns>a transition if found - otherwise null</returns>
        public TransducerTransition GetTransition(int stateID)
        {
            var transition = _transitions.Where(item => item.Value.Item2 == stateID).FirstOrDefault();
            return transition.Equals(default(KeyValuePair<string, Tuple<TransducerTransition, int>>))
                ? null : transition.Value.Item1;
        }

        /// <summary>
        /// Gets the trigger of the first found transition thats leads to a state with the given id number
        /// </summary>
        /// <returns>transition trigger if found - otherwise empty string</returns>
        private string GetTransitionEvent(int stateID)
        {
            var transition = _transitions.Where(item => item.Value.Item2 == stateID).FirstOrDefault();
            return transition.Equals(default(KeyValuePair<string, Tuple<TransducerTransition, int>>))
                ? String.Empty : transition.Key;
        }

        /// <summary>
        /// Returns of list of all transitions starting from this state
        /// </summary>
        public List<TransducerTransition> GetListOfTransitions()
        {
            var result = new List<TransducerTransition>();
            result = _transitions.Select(item => item.Value.Item1.Clone()).ToList();
            return result;
        }

        /// <summary>
        /// Gets the id of the state where the transition - triggered by the given event - leads
        /// </summary>
        /// <returns>State id if such trigger exists, else -1</returns>
        public int GetDestinationIdByTrigger(string trigger)
        {
            if (!_transitions.ContainsKey(trigger)) return -1;
            return _transitions[trigger].Item2;
        }

        /// <summary>
        /// This method will be called when the automaton switches state
        /// </summary>
        /// <param name="action">The action that invokes the switching</param>
        /// <returns>Tuple of the id of state activated by the switching and the generated output. 
        /// Returns null if shifting couldn't be realised - i.e. no such trigger exists or trigger 
        /// condition is not met</returns>
        public Tuple<int, string> ShiftState(TransitionTrigger trigger, double param = -1d)
        {
            if (!_transitions.ContainsKey(trigger.TransitionEvent)) return null;

            var shift = _transitions[trigger.TransitionEvent];
            if (trigger.IsConditional)
            {
                if (!shift.Item1.TransitionTrigger.EvaluateCondition(param)) return null;
            }

            var s = shift.Item1.DoAction();
            return new Tuple<int, string>(shift.Item2, s);
        }

        /// <summary>
        /// Converts the transitions state into it's string representation.
        /// </summary>
        public override string ToString()
        {
            string result = "";
            foreach (var transition in _transitions)
            {
                result += String.Format("{0,-15} E:{1,-15}[{2},{3},{4}] A:{5,-10}-> ", ID, transition.Key.SetStringToLenght(15),
                    transition.Value.Item1.TransitionTrigger.ConditionOperator, transition.Value.Item1.TransitionTrigger.Parameter1,
                    transition.Value.Item1.TransitionTrigger.Parameter2, transition.Value.Item1.ActionName.SetStringToLenght(10));
                result += String.Format("{0,-15} // ", transition.Value.Item2);
                result += String.Format("{0,-15}({1})\n", transition.Value.Item1.Translation.SetStringToLenght(15),
                    transition.Value.Item1.InnovationNumber);
            }
            return result;
        }

    }

}
