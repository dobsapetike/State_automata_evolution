using System;
using System.ComponentModel;

namespace BenchmarkDepot.Classes.Core
{

    /// <summary>
    /// Enum of the operators a transition condition can use
    /// </summary>
    public enum TriggerConditionOperator
    {
        Equals,
        LesserThan,
        GreaterThan,
    }

    /// <summary>
    /// Triggers the state-shifting in the transducer
    /// </summary>
    public struct TransitionTrigger
    {

        #region Properties

        /// <summary>
        /// Gets and sets the string representation of the event activating the transition
        /// </summary>
        public string TransitionEvent
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets whether the activation of this trigger is set to a condition.
        /// It is immutable, constructor sets it's value
        /// </summary>
        public bool IsConditional
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets and sets the condition operator
        /// </summary>
        public TriggerConditionOperator ConditionOperator
        {
            get;
            set;
        }

        private double _parameter = -1d;
        /// <summary>
        /// Gets and sets the condition parameter.
        /// Returns 0 if trigger is not conditional
        /// </summary>
        public double Parameter
        {
            get { return _parameter; }
            set
            {
                if (value > _maxParamValue) value = _maxParamValue;
                if (value < _minParamValue) value = _minParamValue;
                _parameter = value;
            }
        }

        private double _minParamValue = 0d;
        /// <summary>
        /// Gets and sets the min value of the conditional parameter
        /// </summary>
        public double MinParameterValue
        {
            get { return _minParamValue; }
            set
            {
                if (value > _maxParamValue) value = _maxParamValue;
                if (Parameter < value) Parameter = value;
                _minParamValue = value;
            }
        }

        private double _maxParamValue = 100d;
        /// <summary>
        /// Gets and sets the max value of the conditional parameter
        /// </summary>
        public double MaxParameterValue
        {
            get { return _maxParamValue; }
            set
            {
                if (value < _minParamValue) value = _minParamValue;
                if (Parameter > value) Parameter = value;
                _maxParamValue = value;
            }
        }

        #endregion

        #region Constructor

        public TransitionTrigger(string tEvent, bool isCond = false) : this()
        {
            TransitionEvent = tEvent;
            IsConditional = isCond;
        }

        #endregion
        
        #region EvaluteCondition

        /// <summary>
        /// Evaluates the condition with the given value 
        /// </summary>
        /// <returns>whether the value satisfies the condition</returns>
        public bool EvaluateCondition(double value)
        {
            switch (ConditionOperator)
            {
                case TriggerConditionOperator.Equals:
                    return value == Parameter;
                case TriggerConditionOperator.LesserThan:
                    return value < Parameter;
                case TriggerConditionOperator.GreaterThan:
                    return value > Parameter;
            }
            return false;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Triggers are equal if their transition events are equal
        /// </summary>
        public override bool Equals(object obj)
        {
            var a = (TransitionTrigger)obj;
            return a.TransitionEvent == TransitionEvent;
        }

        public override int GetHashCode()
        {
            return TransitionEvent.GetHashCode();
        }

        public static bool operator ==(TransitionTrigger a, TransitionTrigger b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
            return a.TransitionEvent == b.TransitionEvent;
        }

        public static bool operator !=(TransitionTrigger a, TransitionTrigger b)
        {
            return !(a == b);
        }

        #endregion

    }
}
