﻿using System;
using BenchmarkDepot.Classes.Misc;

namespace BenchmarkDepot.Classes.Core.EAlgotihms.Parameters
{

    /// <summary>
    /// General evolutionary algorithm parameters
    /// </summary>
    public class GeneralEAParameters : ObservableObject
    {

        #region Defaults

        const int DefaultInitialPopulationSize = 30;
        const int DefaultMaxPopulationSize = 750;
        const int DefaultMaxIndividualSize = 10; 
        const int DefaultGenerationThreshold = 1500;

        const double DefaultSelectionProportion = 0.25;
        const int DefaultTournamentSize = 2;
        const double DefaultReplacementProportion = 0.85;

        const double DefaultCrossoverProbability = 0.5;
        const double DefaultStateDeletionMutationProbability = 0.25;
        const double DefaultTransitionDeletionMutationProbability = 0.25;
        const double DefaultTransitionActionMutationProbability = 0.25;
        const double DefaultTransitionTranslationMutationProbability = 0.45;
        const double DefaultTransitionTriggerMutationProbability = 0.35;
        const int DefaultMutationCount = 3;

        const bool DefaultElitism = true;

        #endregion

        #region Private fields

        private int _initialPopulationSize;
        private int _maxPopulationSize;
        private int _maxIndividualSize;
        private int _generationThreshold;

        private double _selectionProportion;
        private int _tournamentSize;
        private double _replacementProportion;

        private double _crossoverProbability;
        private double _stateDeletionMutationProbability;
        private double _transitionDeletionMutationProbability;
        private double _transitionActionMutationProbability;
        private double _transitionTranslationMutationProbability;
        private double _transitionTriggerMutationProbability;
        private int _mutationCount;

        private bool _elitism;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the initial population size - before the start of
        /// the evolutionary cycle
        /// </summary>
        public int InitialPopulationSize
        {
            get { return _initialPopulationSize; }
            set 
            { 
                _initialPopulationSize = value > MaxPopulationSize ? MaxPopulationSize : value;
                RaisePropertyChanged(() => InitialPopulationSize);
            }
        }

        /// <summary>
        /// Gets and sets the maximum number of individuals in the population
        /// </summary>
        public int MaxPopulationSize
        {
            get { return _maxPopulationSize; }
            set 
            { 
                _maxPopulationSize = value;
                RaisePropertyChanged(() => MaxPopulationSize);
            }
        }

        /// <summary>
        /// Gets and sets the maximum number of state in a transducer
        /// </summary>
        public int MaxIndividualSize
        {
            get { return _maxIndividualSize; }
            set 
            {
                _maxIndividualSize = value;
                RaisePropertyChanged(() => MaxIndividualSize);
            }
        }

        /// <summary>
        /// Gets and sets the generation threshold - ie. the maximum number of 
        /// generations
        /// </summary>
        public int GenerationThreshold
        {
            get { return _generationThreshold; }
            set 
            { 
                _generationThreshold = value;
                RaisePropertyChanged(() => GenerationThreshold);
            }
        }

        /// <summary>
        /// Gets and sets the selection proportion - what percent of the population
        /// is going to become potential parents for producing offsprings
        /// </summary>
        public double SelectionProportion
        {
            get { return _selectionProportion; }
            set 
            { 
                _selectionProportion = value > 1d ? 1d : value;
                RaisePropertyChanged(() => SelectionProportion);
            }
        }
        
        /// <summary>
        /// Gets and sets the the number of individuals selected
        /// during the tournament selection
        /// </summary>
        public int TournamentSize
        {
            get { return _tournamentSize; }
            set
            {
                _tournamentSize = value;
                RaisePropertyChanged(() => TournamentSize);
            }
        }

        /// <summary>
        /// Gets and sets the replacement proportion - what percent of population
        /// is going to be replaced
        /// </summary>
        public double ReplacementProportion
        {
            get { return _replacementProportion; }
            set 
            { 
                _replacementProportion = value > 1d ? 1d : value;
                RaisePropertyChanged(() => ReplacementProportion);
            }
        }

        /// <summary>
        /// Gets and sets the probability, that a new individual will be
        /// created via crossover
        /// </summary>
        public double CrossoverProbability
        {
            get { return _crossoverProbability; }
            set 
            { 
                _crossoverProbability = value > 1d ? 1d : value;
                RaisePropertyChanged(() => CrossoverProbability);
            }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random state is deleted
        /// </summary>
        public double StateDeletionMutationProbability
        {
            get { return _stateDeletionMutationProbability; }
            set 
            { 
                _stateDeletionMutationProbability = value > 1d ? 1d : value;
                RaisePropertyChanged(() => StateDeletionMutationProbability);
            }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition is deleted
        /// </summary>
        public double TransitionDeletionMutationProbability
        {
            get { return _transitionDeletionMutationProbability; }
            set 
            { 
                _transitionDeletionMutationProbability = value > 1d ? 1d : value;
                RaisePropertyChanged(() => TransitionDeletionMutationProbability);
            }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition action is modified
        /// </summary>
        public double TransitionActionMutationProbability
        {
            get { return _transitionActionMutationProbability; }
            set 
            { 
                _transitionActionMutationProbability = value > 1d ? 1d : value;
                RaisePropertyChanged(() => TransitionActionMutationProbability);
            }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition translation is modified
        /// </summary>
        public double TransitionTranslationMutationProbability
        {
            get { return _transitionTranslationMutationProbability; }
            set 
            { 
                _transitionTranslationMutationProbability = value > 1d ? 1d : value;
                RaisePropertyChanged(() => TransitionTranslationMutationProbability);
            }
        }

        /// <summary>
        /// Gets and sets the probability of mutation when a random transition trigger is modified
        /// </summary>
        public double TransitionTriggerMutationProbability
        {
            get { return _transitionTriggerMutationProbability; }
            set 
            { 
                _transitionTriggerMutationProbability = value > 1d ? 1d : value;
                RaisePropertyChanged(() => TransitionTriggerMutationProbability);
            }
        }

        /// <summary>
        /// Gets and sets how many times a new individual can be mutated
        /// </summary>
        public int MutationCount
        {
            get { return _mutationCount; }
            set
            {
                _mutationCount = value > 0 ? value : 1;
                RaisePropertyChanged(() => MutationCount);
            }
        }

        /// <summary>
        /// Gets and sets whether elitism is turned on
        /// </summary>
        public bool Elitism
        {
            get { return _elitism; }
            set
            {
                _elitism = value;
                RaisePropertyChanged(() => Elitism);
            }
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
            MaxIndividualSize = DefaultMaxIndividualSize;
            GenerationThreshold = DefaultGenerationThreshold;

            Elitism = DefaultElitism;
            SelectionProportion = DefaultSelectionProportion;
            TournamentSize = DefaultTournamentSize;
            ReplacementProportion = DefaultReplacementProportion;

            CrossoverProbability = DefaultCrossoverProbability;
            StateDeletionMutationProbability = DefaultStateDeletionMutationProbability;
            TransitionDeletionMutationProbability = DefaultTransitionDeletionMutationProbability;
            TransitionActionMutationProbability = DefaultTransitionActionMutationProbability;
            TransitionTranslationMutationProbability = DefaultTransitionTranslationMutationProbability;
            TransitionTriggerMutationProbability = DefaultTransitionTriggerMutationProbability;
            MutationCount = DefaultMutationCount;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets variable values from a preset
        /// </summary>
        public void LoadFromPreset(Preset preset)
        {
            MaxPopulationSize = preset.MaxPopulationSize;
            InitialPopulationSize = preset.InitialPopulationSize;
            MaxIndividualSize = preset.MaxIndividualSize;
            GenerationThreshold = preset.GenerationThreshold;
            Elitism = preset.Elitism;
            SelectionProportion = preset.SelectionProportion;
            TournamentSize = preset.TournamentSize;
            ReplacementProportion = preset.ReplacementProportion;
            CrossoverProbability = preset.CrossoverProbability;
            StateDeletionMutationProbability = preset.StateDeletionMutationProbability;
            TransitionDeletionMutationProbability = preset.TransitionDeletionMutationProbability;
            TransitionActionMutationProbability = preset.TransitionActionMutationProbability;
            TransitionTranslationMutationProbability = preset.TransitionTranslationMutationProbability;
            TransitionTriggerMutationProbability = preset.TransitionTriggerMutationProbability;
            MutationCount = preset.MutationCount;
        }

        #endregion

    }

}
