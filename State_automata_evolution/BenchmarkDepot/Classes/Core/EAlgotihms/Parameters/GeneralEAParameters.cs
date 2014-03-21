using System;

namespace BenchmarkDepot.Classes.Core.EAlgotihms.Parameters
{

    /// <summary>
    /// General evolutionary algorithm parameters
    /// </summary>
    public class GeneralEAParameters
    {

        #region Defaults

        const int DefaultInitialPopulationSize = 30;
        const int DefaultMaxPopulationSize = 750;
        const int DefaultGenerationThreshold = 1500;

        const double DefaultSelectionProportion = 0.25;
        const double DefaultMutationProportion = 0.25;
        const double DefaultReplacementProportion = 0.85;

        const double DefaultCrossoverProbability = 0.5;
        const double DefaultStateDeletionMutationProbability = 0.25;
        const double DefaultTransitionDeletionMutationProbability = 0.25;
        const double DefaultTransitionActionMutationProbability = 0.25;
        const double DefaultTransitionTriggerMutationProbability = 0.35;

        #endregion

        #region Private fields

        private int _initialPopulationSize;
        private int _maxPopulationSize;
        private int _generationThreshold;

        private double _selectionProportion;
        private double _mutationProportion;
        private double _replacementProportion;

        private double _crossoverProbability;
        private double _stateDeletionMutationProbability;
        private double _transitionDeletionMutationProbability;
        private double _transitionActionMutationProbability;
        private double _transitionTriggerMutationProbability;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the initial population size - before the start of
        /// the evolutionary cycle
        /// </summary>
        public int InitialPopulationSize
        {
            get { return _initialPopulationSize; }
            set { _initialPopulationSize = value; }
        }

        /// <summary>
        /// Gets and sets the maximum number of individuals in the population
        /// </summary>
        public int MaxPopulationSize
        {
            get { return _maxPopulationSize; }
            set { _maxPopulationSize = value; }
        }

        /// <summary>
        /// Gets and sets the generation threshold - ie. the maximum number of 
        /// generations
        /// </summary>
        public int GenerationThreshold
        {
            get { return _generationThreshold; }
            set { _generationThreshold = value; }
        }

        /// <summary>
        /// Gets and sets the selection proportion - what percent of the population
        /// is going to become parents for producing offsprings
        /// </summary>
        public double SelectionProportion
        {
            get { return _selectionProportion; }
            set { _selectionProportion = value; }
        }

        /// <summary>
        /// Gets and sets the mutation proportion - what percent of population
        /// is going to mutate in one generation
        /// </summary>
        public double MutationProportion
        {
            get { return _mutationProportion; }
            set { _mutationProportion = value; }
        }

        /// <summary>
        /// Gets and sets the replacement proportion - what top percent of population
        /// is going to the next generation
        /// </summary>
        public double ReplacementProportion
        {
            get { return _replacementProportion; }
            set { _replacementProportion = value; }
        }

        /// <summary>
        /// Gets and sets the probability of crossover between two individuals
        /// </summary>
        public double CrossoverProbability
        {
            get { return _crossoverProbability; }
            set { _crossoverProbability = value; }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random state is deleted
        /// </summary>
        public double StateDeletionMutationProbability
        {
            get { return _stateDeletionMutationProbability; }
            set { _stateDeletionMutationProbability = value; }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition is deleted
        /// </summary>
        public double TransitionDeletionMutationProbability
        {
            get { return _transitionDeletionMutationProbability; }
            set { _transitionDeletionMutationProbability = value; }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition action is modified
        /// </summary>
        public double TransitionActionMutationProbability
        {
            get { return _transitionActionMutationProbability; }
            set { _transitionActionMutationProbability = value; }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition trigger is modified
        /// </summary>
        public double TransitionTriggerMutationProbability
        {
            get { return _transitionTriggerMutationProbability; }
            set { _transitionTriggerMutationProbability = value; }
        }
        
        #endregion

        #region Constuctor

        /// <summary>
        /// Constructor sets the dafult value for every parameters
        /// </summary>
        public GeneralEAParameters()
        {
            InitialPopulationSize = DefaultInitialPopulationSize;
            MaxPopulationSize = DefaultMaxPopulationSize;
            GenerationThreshold = DefaultGenerationThreshold;

            SelectionProportion = DefaultSelectionProportion;
            MutationProportion = DefaultMutationProportion;
            ReplacementProportion = DefaultReplacementProportion;

            CrossoverProbability = DefaultCrossoverProbability;
            StateDeletionMutationProbability = DefaultStateDeletionMutationProbability;
            TransitionDeletionMutationProbability = DefaultTransitionDeletionMutationProbability;
            TransitionActionMutationProbability = DefaultTransitionActionMutationProbability;
            TransitionTriggerMutationProbability = DefaultTransitionTriggerMutationProbability;
        }

        #endregion

    }

}
