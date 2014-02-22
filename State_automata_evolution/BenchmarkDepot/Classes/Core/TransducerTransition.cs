﻿using System;

namespace BenchmarkDepot.Classes.Core
{

    /// <summary>
    /// Class reprezentation of a transition between two transducer states
    /// </summary>
    public class TransducerTransition : ICloneable
    {

        /// <summary>
        /// Each transition has it's own action assigned, which is executed
        /// while the automaton is shifting from one of it's incidental states to another
        /// </summary>
        private Action _transitionAction;

        /// <summary>
        /// The output generated by switching states through this transition
        /// </summary>
        private string _transitionTranslation;
        public string Translation
        {
            get { return _transitionTranslation; }
        }

        /// <summary>
        /// The event activating this transition
        /// </summary>
        private string _transitionEvent;
        public string TransitionEvent
        {
            get { return _transitionEvent; }
            set { _transitionEvent = value; }
        }

        /// <summary>
        /// Innovation number used by NEAT evolution for detecting historical origin  
        /// </summary>
        private int _innovationNumber;
        public int InnovationNumber
        {
            get { return _innovationNumber; }
        }

        /// <summary>
        /// Id of transducer state from where the transition start
        /// </summary>
        private int _stateFrom;
        public int StateFrom
        {
            get { return _stateFrom; }
            set { _stateFrom = value; }
        }

        /// <summary>
        /// Id of transducer state where the transition leads to
        /// </summary>
        private int _stateTo;
        public int StateTo
        {
            get { return _stateTo; }
            set { _stateTo = value; }
        }

        #region Constructor

        public TransducerTransition(Action action, string translation, int innovationNumber)
        {
            _transitionAction = action;
            _transitionTranslation = translation;
            _innovationNumber = innovationNumber;
        }

        #endregion

        #region Cloning

        public object Clone()
        {
            var other = (TransducerTransition)this.MemberwiseClone();
            if (_transitionAction != null)
            {
                other._transitionAction = new Action(_transitionAction);
            }
            return other;
        }

        #endregion

        /// <summary>
        /// Invokes the transition action
        /// </summary>
        /// <returns>Generated output</returns>
        public string DoAction()
        {
            if (_transitionAction != null)
            {
                _transitionAction.Invoke();
            }
            return _transitionTranslation;
        }

    }

}
