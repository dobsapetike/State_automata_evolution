using System;

namespace BenchmarkDepot.Classes.Core.EAlgotihms.Parameters
{

    /// <summary>
    /// Parameters specific to NEAT evolution
    /// </summary>
    public class NEATParameters
    {

        #region Defaults

        const int DefaultMaxSpecieCount = -1;
        const double DefaultCompatibilityThreshold = 3.0;

        const double DefaultCoefExcessGeneFactor = 1.0;
        const double DefaultCoefDisjointGeneFactor = 1.0;
        const double DefaultCoefMatchinWeightDifferenceFactor = 0.3;
        const double DefaultMatchingWeightDifferenceValue = 1.0;

        const double DefaultAddNodeMutationProbability = 0.25;
        const double DefaultAddTransitionMutationProbability = 0.25;

        #endregion

        #region Private fields

        private int _maxSpecieCount;
        private double _compatibilityThreshold;

        private double _coefExcessGeneFactor;
        private double _coefDisjointGeneFactor;
        private double _coefMatchingWeightDifferenceFactor;
        private double _matchingWeightDifferenceValue;

        private double _addNodeMutationProbability;
        private double _addTransitionMutationProbability;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the maximum number of species
        /// If it's value is lower than 1 this parameter gets ignored, which
        /// means unlimited amount of species
        /// </summary>
        public int MaxSpecieCount
        {
            get { return _maxSpecieCount; }
            set { _maxSpecieCount = value; }
        }

        /// <summary>
        /// Gets and sets the compatibility threshold for speciation
        /// </summary>
        public double CompatibilityThreshold
        {
            get { return _compatibilityThreshold; }
            set { _compatibilityThreshold = value; }
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

        #endregion

        #region Constuctor

        /// <summary>
        /// Constructor sets the dafult value for all parameters
        /// </summary>
        public NEATParameters()
        {
            _maxSpecieCount = DefaultMaxSpecieCount;
            _compatibilityThreshold = DefaultCompatibilityThreshold;

            _coefExcessGeneFactor = DefaultCoefExcessGeneFactor;
            _coefDisjointGeneFactor = DefaultCoefDisjointGeneFactor;
            _coefMatchingWeightDifferenceFactor = DefaultCoefMatchinWeightDifferenceFactor;
            _matchingWeightDifferenceValue = DefaultMatchingWeightDifferenceValue;

            _addNodeMutationProbability = DefaultAddNodeMutationProbability;
            _addTransitionMutationProbability = DefaultAddTransitionMutationProbability;
        }

        #endregion

    }

}
