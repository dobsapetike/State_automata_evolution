using System;
using BenchmarkDepot.Classes.Misc;

namespace BenchmarkDepot.Classes.Core.EAlgotihms.Parameters
{

    /// <summary>
    /// Parameters specific to NEAT evolution
    /// </summary>
    public class NEATParameters : ObservableObject
    {

        #region Defaults

        const bool DefaultSpeciesAllowed = true;
        const int DefaultRelativeMaxSpecieCount = 50;
        const int DefaultSpeciesAllowedStagnatedGenerationCount = 3;
        const double DefaultCompatibilityThreshold = 3.0;
        const double DefaultMinCompatibilityThreshold = 0.5;
        const double DefaultCompatibilityThresholdDelta = 0.2;

        const double DefaultCoefExcessGeneFactor = 1.0;
        const double DefaultCoefDisjointGeneFactor = 1.0;
        const double DefaultCoefMatchinWeightDifferenceFactor = 1.0;
        const double DefaultMatchingWeightDifferenceValue = 1.0;

        const double DefaultAddNodeMutationProbability = 0.25;
        const double DefaultAddTransitionMutationProbability = 0.25;

        const double DefaultSurvivalRate = 0.5;

        const bool DefaultInnovationResetPerGeneration = true;

        #endregion

        #region Private fields

        private bool _speciesAllowed;
        private int _maxRelativeSpeciesCount;
        private int _allowedSpeciesStagnatedGenerationCount;
        private double _compatibilityThreshold;
        private double _minCompatibilityThreshold;
        private double _compatibilityThresholdDelta;

        private double _coefExcessGeneFactor;
        private double _coefDisjointGeneFactor;
        private double _coefMatchingWeightDifferenceFactor;
        private double _matchingWeightDifferenceValue;

        private double _addNodeMutationProbability;
        private double _addTransitionMutationProbability;

        private double _survivalRate;

        private bool _innovationResetPerGeneration;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets whether speciation is allowed
        /// </summary>
        public bool SpeciesAllowed
        {
            get { return _speciesAllowed; }
            set
            {
                MinCompatibilityThreshold = value 
                    ? DefaultMinCompatibilityThreshold 
                    : Double.PositiveInfinity; // this ensures each individuals gets into the same species
                _speciesAllowed = value;
            }
        }

        /// <summary>
        /// Gets and sets the relative maximum number of species.
        /// Note, that it doesn't garantee that specie count won't be higher at a given point of the
        /// computation - but the algorithm will try to keep their size below this value
        /// </summary>
        public int MaxRelativeSpecieCount
        {
            get { return _maxRelativeSpeciesCount; }
            set { _maxRelativeSpeciesCount = value; }
        }

        /// <summary>
        /// Gets and sets the maximum number of generations a species is allowed to survive
        /// without any kind of improvement
        /// </summary>
        public int AllowedSpeciesStagnatedGenerationCount
        {
            get { return _allowedSpeciesStagnatedGenerationCount; }
            set { _allowedSpeciesStagnatedGenerationCount = value; }
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
                    return;
                }
                _compatibilityThreshold = value;
                RaisePropertyChanged(() => CompatibilityThreshold);
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
                _minCompatibilityThreshold = value;
                if (_compatibilityThreshold < value)
                {
                    _compatibilityThreshold = value;
                }
            }
        }

        /// <summary>
        /// Gets and sets the threshold delta.
        /// The algorithm dynamically changes the compatibility threshold based on the number 
        /// of species. This delta sets by how much per change.
        /// </summary>
        public double CompatibilityThresholdDelta
        {
            get { return _compatibilityThresholdDelta; }
            set { _compatibilityThresholdDelta = value; }
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
            set { _addNodeMutationProbability = value > 1d ? 1d : value; ; }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition is added
        /// </summary>
        public double AddTransitionMutationProbability
        {
            get { return _addTransitionMutationProbability; }
            set { _addTransitionMutationProbability = value > 1d ? 1d : value; ; }
        }

        /// <summary>
        /// Gets and sets what top percent of the species is qualified for 
        /// mutation/crossover
        /// </summary>
        public double SurvivalRate
        {
            get { return _survivalRate; }
            set { _survivalRate = value > 1d ? 1d : value; ; }
        }

        /// <summary>
        /// Gets and sets whether the list of every occurred innovations should reset
        /// at the end of each generation
        /// </summary>
        public bool InnovationResetPerGeneration
        {
            get { return _innovationResetPerGeneration; }
            set { _innovationResetPerGeneration = value; }
        }

        #endregion

        #region Constuctor

        /// <summary>
        /// Constructor sets the dafult value for every parameters
        /// </summary>
        public NEATParameters()
        {
            _speciesAllowed = DefaultSpeciesAllowed;
            _maxRelativeSpeciesCount = DefaultRelativeMaxSpecieCount;
            _allowedSpeciesStagnatedGenerationCount = DefaultSpeciesAllowedStagnatedGenerationCount;
            _compatibilityThreshold = DefaultCompatibilityThreshold;
            _minCompatibilityThreshold = DefaultMinCompatibilityThreshold;
            _compatibilityThresholdDelta = DefaultCompatibilityThresholdDelta;

            _coefExcessGeneFactor = DefaultCoefExcessGeneFactor;
            _coefDisjointGeneFactor = DefaultCoefDisjointGeneFactor;
            _coefMatchingWeightDifferenceFactor = DefaultCoefMatchinWeightDifferenceFactor;
            _matchingWeightDifferenceValue = DefaultMatchingWeightDifferenceValue;

            _addNodeMutationProbability = DefaultAddNodeMutationProbability;
            _addTransitionMutationProbability = DefaultAddTransitionMutationProbability;

            _survivalRate = DefaultSurvivalRate;

            _innovationResetPerGeneration = DefaultInnovationResetPerGeneration;
        }

        #endregion

    }

}
