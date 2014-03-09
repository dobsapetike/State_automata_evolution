using System;

namespace BenchmarkDepot.Classes.Core.EAlgotihms.Parameters
{

    /// <summary>
    /// Parameters specific to NEAT evolution
    /// </summary>
    public class NEATParameters
    {

        #region Defaults

        const int DefaultMaxSpecieSize = -1;
        const double DefaultCompatibilityThreshold = 3.0;
        const double DefaultMinCompatibilityThreshold = 0.5;

        const double DefaultCoefExcessGeneFactor = 1.0;
        const double DefaultCoefDisjointGeneFactor = 1.0;
        const double DefaultCoefMatchinWeightDifferenceFactor = 1.0;
        const double DefaultMatchingWeightDifferenceValue = 1.0;

        const double DefaultAddNodeMutationProbability = 0.25;
        const double DefaultAddTransitionMutationProbability = 0.25;

        const double DefaultSurvivalRate = 0.5;

        #endregion

        #region Private fields

        private int _maxSpecieSize;
        private double _compatibilityThreshold;
        private double _minCompatibilityThreshold;

        private double _coefExcessGeneFactor;
        private double _coefDisjointGeneFactor;
        private double _coefMatchingWeightDifferenceFactor;
        private double _matchingWeightDifferenceValue;

        private double _addNodeMutationProbability;
        private double _addTransitionMutationProbability;

        private double _survivalRate;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the maximum number of individuals in a species
        /// </summary>
        public int MaxSpecieSize
        {
            get { return _maxSpecieSize; }
            set { _maxSpecieSize = value; }
        }

        /// <summary>
        /// Gets and sets the compatibility threshold for speciation
        /// </summary>
        public double CompatibilityThreshold
        {
            get { return _compatibilityThreshold; }
            set 
            {
                if (value < _minCompatibilityThreshold)
                {
                    _compatibilityThreshold = _minCompatibilityThreshold;
                }
                _compatibilityThreshold = value; 
            }
        }

        /// <summary>
        /// Gets and sets the minimum value of the compatibility threshold
        /// </summary>
        public double MinCompatibilityThreshold
        {
            get { return _minCompatibilityThreshold; }
            set
            {
                if (_compatibilityThreshold < value)
                {
                    _compatibilityThreshold = value;
                }
                _minCompatibilityThreshold = value;
            }
        }

        /// <summary>
        /// Gets and sets the coeficient for setting the importance of the 
        /// number of excess genes in the formula for computing the compatibility distance
        /// </summary>
        public double CoefExcessGeneFactor
        {
            get { return _coefExcessGeneFactor; }
            set { _coefExcessGeneFactor = value; }
        }

        /// <summary>
        /// Gets and sets the coeficient for setting the importance of the 
        /// number of disjoint genes in the formula for computing the compatibility distance
        /// </summary>
        public double CoefDisjointGeneFactor
        {
            get { return _coefDisjointGeneFactor; }
            set { _coefDisjointGeneFactor = value; }
        }

        /// <summary>
        /// Gets and sets the coeficient for setting the importance of the 
        /// weight difference of matching genes in the formula for computing the compatibility distance
        /// </summary>
        public double CoefMatchingWeightDifferenceFactor
        {
            get { return _coefMatchingWeightDifferenceFactor; }
            set { _coefMatchingWeightDifferenceFactor = value; }
        }

        /// <summary>
        /// Gets and sets the value used instead of weight difference, in this case when
        /// the transition action and trigger are the same
        /// In case of one is mismatched the value is halved
        /// </summary>
        public double MatchingWeightDifferenceValue
        {
            get { return _matchingWeightDifferenceValue; }
            set { _matchingWeightDifferenceValue = value; }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random node(state) is added
        /// </summary>
        public double AddNodeMutationProbability
        {
            get { return _addNodeMutationProbability; }
            set { _addNodeMutationProbability = value; }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition is added
        /// </summary>
        public double AddTransitionMutationProbability
        {
            get { return _addTransitionMutationProbability; }
            set { _addTransitionMutationProbability = value; }
        }

        /// <summary>
        /// Gets and sets what top percent of the species is qualified for 
        /// mutation/crossover
        /// </summary>
        public double SurvivalRate
        {
            get { return _survivalRate; }
            set { _survivalRate = value; }
        }

        #endregion

        #region Constuctor

        /// <summary>
        /// Constructor sets the dafult value for all parameters
        /// </summary>
        public NEATParameters()
        {
            _maxSpecieSize = DefaultMaxSpecieSize;
            _compatibilityThreshold = DefaultCompatibilityThreshold;
            _minCompatibilityThreshold = DefaultMinCompatibilityThreshold;

            _coefExcessGeneFactor = DefaultCoefExcessGeneFactor;
            _coefDisjointGeneFactor = DefaultCoefDisjointGeneFactor;
            _coefMatchingWeightDifferenceFactor = DefaultCoefMatchinWeightDifferenceFactor;
            _matchingWeightDifferenceValue = DefaultMatchingWeightDifferenceValue;

            _addNodeMutationProbability = DefaultAddNodeMutationProbability;
            _addTransitionMutationProbability = DefaultAddTransitionMutationProbability;

            _survivalRate = DefaultSurvivalRate;
        }

        #endregion

    }

}
