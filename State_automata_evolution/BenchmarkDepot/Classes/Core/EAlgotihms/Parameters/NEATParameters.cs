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
        const int DefaultCriticalSpecieCount = 50;
        const int DefaultSpeciesAllowedStagnatedGenerationCount = 3;
        const double DefaultCompatibilityThreshold = 0.5;
        const double DefaultMinCompatibilityThreshold = 0.1;
        const double DefaultCompatibilityThresholdDelta = 0.02;
        const bool DefaultUseNormalizedRepresentant = false;

        const double DefaultCoefExcessGeneFactor = 1.0;
        const double DefaultCoefDisjointGeneFactor = 1.0;
        const double DefaultCoefMatchinWeightDifferenceFactor = 1.0;
        const double DefaultMatchingWeightDifferenceValue = 1.0;

        const double DefaultAddNodeMutationProbability = 0.25;
        const double DefaultAddTransitionMutationProbability = 0.25;

        const bool DefaultInnovationResetPerGeneration = true;

        #endregion

        #region Private fields

        private bool _speciesAllowed;
        private int _criticalSpeciesCount;
        private int _allowedSpeciesStagnatedGenerationCount;
        private double _compatibilityThreshold;
        private double _minCompatibilityThreshold;
        private double _compatibilityThresholdDelta;
        private bool _useNormalizedRepresentant;

        private double _coefExcessGeneFactor;
        private double _coefDisjointGeneFactor;
        private double _coefMatchingWeightDifferenceFactor;
        private double _matchingWeightDifferenceValue;

        private double _addNodeMutationProbability;
        private double _addTransitionMutationProbability;

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
                CompatibilityThreshold = DefaultCompatibilityThreshold;
                _speciesAllowed = value;
                RaisePropertyChanged(() => SpeciesAllowed);
            }
        }

        /// <summary>
        /// Gets and sets the critical number of species.
        /// Note, that it doesn't garantee that specie count won't be higher at a given point of the
        /// computation - but the algorithm will try to keep their size below this value
        /// </summary>
        public int CriticalSpecieCount
        {
            get { return _criticalSpeciesCount; }
            set 
            { 
                _criticalSpeciesCount = value;
                RaisePropertyChanged(() => CriticalSpecieCount);
            }
        }

        /// <summary>
        /// Gets and sets the maximum number of generations a species is allowed to survive
        /// without any kind of improvement
        /// </summary>
        public int AllowedSpeciesStagnatedGenerationCount
        {
            get { return _allowedSpeciesStagnatedGenerationCount; }
            set 
            { 
                _allowedSpeciesStagnatedGenerationCount = value;
                RaisePropertyChanged(() => AllowedSpeciesStagnatedGenerationCount);
            }
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
                _compatibilityThreshold = value > 1d ? 1d : value;
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
                RaisePropertyChanged(() => MinCompatibilityThreshold);
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
            set 
            { 
                _compatibilityThresholdDelta = value;
                RaisePropertyChanged(() => CompatibilityThresholdDelta);
            }
        }

        /// <summary>
        /// Gets and sets the coeficient for setting the importance of the 
        /// number of excess genes in the formula for computing the compatibility distance
        /// </summary>
        public double CoefExcessGeneFactor
        {
            get { return _coefExcessGeneFactor; }
            set 
            { 
                _coefExcessGeneFactor = value;
                RaisePropertyChanged(() => CoefExcessGeneFactor);
            }
        }

        /// <summary>
        /// Gets and sets the coeficient for setting the importance of the 
        /// number of disjoint genes in the formula for computing the compatibility distance
        /// </summary>
        public double CoefDisjointGeneFactor
        {
            get { return _coefDisjointGeneFactor; }
            set 
            {
                _coefDisjointGeneFactor = value;
                RaisePropertyChanged(() => CoefDisjointGeneFactor);
            }
        }

        /// <summary>
        /// Gets and sets the coeficient for setting the importance of the 
        /// weight difference of matching genes in the formula for computing the compatibility distance
        /// </summary>
        public double CoefMatchingWeightDifferenceFactor
        {
            get { return _coefMatchingWeightDifferenceFactor; }
            set 
            { 
                _coefMatchingWeightDifferenceFactor = value;
                RaisePropertyChanged(() => CoefMatchingWeightDifferenceFactor);
            }
        }

        /// <summary>
        /// Gets and sets the value used instead of weight difference, in this case when
        /// the transition action and trigger are the same
        /// In case of one is mismatched the value is halved
        /// </summary>
        public double MatchingWeightDifferenceValue
        {
            get { return _matchingWeightDifferenceValue; }
            set 
            { 
                _matchingWeightDifferenceValue = value;
                RaisePropertyChanged(() => MatchingWeightDifferenceValue);
            }
        }

        /// <summary>
        /// Whether the algoritm should use the best individual as representant 
        /// or a normalized version
        /// </summary>
        public bool UseNormalizedRepresentant
        {
            get { return _useNormalizedRepresentant; }
            set 
            {
                _useNormalizedRepresentant = value;
                RaisePropertyChanged(() => UseNormalizedRepresentant);
            }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random node(state) is added
        /// </summary>
        public double AddNodeMutationProbability
        {
            get { return _addNodeMutationProbability; }
            set 
            { 
                _addNodeMutationProbability = value > 1d ? 1d : value;
                RaisePropertyChanged(() => AddNodeMutationProbability);
            }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition is added
        /// </summary>
        public double AddTransitionMutationProbability
        {
            get { return _addTransitionMutationProbability; }
            set 
            { 
                _addTransitionMutationProbability = value > 1d ? 1d : value;
                RaisePropertyChanged(() => AddTransitionMutationProbability);
            }
        }


        /// <summary>
        /// Gets and sets whether the list of every occurred innovations should reset
        /// at the end of each generation
        /// </summary>
        public bool InnovationResetPerGeneration
        {
            get { return _innovationResetPerGeneration; }
            set 
            { 
                _innovationResetPerGeneration = value;
                RaisePropertyChanged(() => InnovationResetPerGeneration);
            }
        }

        #endregion

        #region Constuctor

        /// <summary>
        /// Constructor sets the dafult value for every parameters
        /// </summary>
        public NEATParameters()
        {
            _speciesAllowed = DefaultSpeciesAllowed;
            _criticalSpeciesCount = DefaultCriticalSpecieCount;
            _allowedSpeciesStagnatedGenerationCount = DefaultSpeciesAllowedStagnatedGenerationCount;
            _compatibilityThreshold = DefaultCompatibilityThreshold;
            _minCompatibilityThreshold = DefaultMinCompatibilityThreshold;
            _compatibilityThresholdDelta = DefaultCompatibilityThresholdDelta;
            _useNormalizedRepresentant = DefaultUseNormalizedRepresentant;

            _coefExcessGeneFactor = DefaultCoefExcessGeneFactor;
            _coefDisjointGeneFactor = DefaultCoefDisjointGeneFactor;
            _coefMatchingWeightDifferenceFactor = DefaultCoefMatchinWeightDifferenceFactor;
            _matchingWeightDifferenceValue = DefaultMatchingWeightDifferenceValue;

            _addNodeMutationProbability = DefaultAddNodeMutationProbability;
            _addTransitionMutationProbability = DefaultAddTransitionMutationProbability;

            _innovationResetPerGeneration = DefaultInnovationResetPerGeneration;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets variable values from a preset
        /// </summary>
        public void LoadFromPreset(Preset preset)
        {
            SpeciesAllowed = preset.SpeciesAllowed;
            CriticalSpecieCount = preset.CriticalSpecieCount;
            AllowedSpeciesStagnatedGenerationCount = preset.AllowedSpeciesStagnatedGenerationCount;
            CompatibilityThreshold = preset.CompatibilityThreshold;
            MinCompatibilityThreshold = preset.MinCompatibilityThreshold;
            CompatibilityThresholdDelta = preset.CompatibilityThresholdDelta;
            CoefExcessGeneFactor = preset.CoefExcessGeneFactor;
            CoefDisjointGeneFactor = preset.CoefExcessGeneFactor;
            CoefMatchingWeightDifferenceFactor = preset.CoefMatchingWeightDifferenceFactor;
            MatchingWeightDifferenceValue = preset.MatchingWeightDifferenceValue;
            UseNormalizedRepresentant = preset.UseNormalizedRepresentant;
            AddNodeMutationProbability = preset.AddNodeMutationProbability;
            AddTransitionMutationProbability = preset.AddTransitionMutationProbability;
            InnovationResetPerGeneration = preset.InnovationResetPerGeneration;
        }

        #endregion

    }

}
