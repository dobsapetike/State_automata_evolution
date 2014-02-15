using System;

namespace BenchmarkDepot.Classes.Core.EAlgotihms.Parameters
{

    /// <summary>
    /// General evolutionary algorithm parameters
    /// </summary>
    public class GeneralEAParameters
    {

        #region Defaults

        const int DefaultInitialPopulationSize = 25;
        const int DefaultMaxPopulationSize = 250;
        const int DefaultGenerationThreshold = 1500;

        const double DefaultSelectionProportion = 0.25;
        const double DefaultMutationProportion = 0.25;
        const double DefaultReplacementProportion = 0.85;

        #endregion

        #region Private fields

        private int _initialPopulationSize;
        private int _maxPopulationSize;
        private int _generationThreshold;

        private double _selectionProportion;
        private double _mutationProportion;
        private double _replacementProportion;

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

        
        #endregion

        #region Constuctor

        public GeneralEAParameters()
        {
            InitialPopulationSize = DefaultInitialPopulationSize;
            MaxPopulationSize = DefaultMaxPopulationSize;
            GenerationThreshold = DefaultGenerationThreshold;

            SelectionProportion = DefaultSelectionProportion;
            MutationProportion = DefaultMutationProportion;
            ReplacementProportion = DefaultReplacementProportion;
        }

        #endregion

    }

}
