using System;
using System.Collections.Generic;
using System.Linq;
using Misc = BenchmarkDepot.Classes.Misc;
using BenchmarkDepot.Classes.Misc;
using BenchmarkDepot.Classes.Core.EAlgotihms.Parameters;
using BenchmarkDepot.Classes.Core.Interfaces;

namespace BenchmarkDepot.Classes.Core.EAlgotihms
{

    /// <summary>
    /// Full NEAT algorithm with all of it's features - also a base class for the other
    /// evolutionary algorithms, which are basically the same with some ablations
    /// </summary>
    public class NEATAlgorithm : IEvolutionaryAlgorithm
    {

        #region Private/protected fields

        #region Counters

        /// <summary>
        /// Number of current generation
        /// </summary>
        protected int _generation = 0;

        /// <summary>
        /// Counter for state id
        /// </summary>
        protected int _stateIndex = 0;

        /// <summary>
        /// Counter for species id
        /// </summary>
        protected int _speciesIndex = 0;

        /// <summary>
        /// Counter for innovation id
        /// </summary>
        protected int _globalInnovationNumber = 0;

        #endregion

        #region Parameters

        protected NEATParameters _neatParameters;
        protected GeneralEAParameters _generalParameters;

        #endregion

        /// <summary>
        /// List of all innovations that occurred during the current generation
        /// </summary>
        protected List<Innovation> _innovations;

        /// <summary>
        /// List of all species in the population
        /// </summary>
        protected List<Species> _species;

        /// <summary>
        /// Sorted collection of all individuals in the population
        /// </summary>
        protected Misc.SortedList<Transducer> _population;

        private Random random;

        #endregion

        #region Properties

        #region Name

        /// <summary>
        /// Returns the name of the algorithm
        /// </summary>
        public virtual string Name
        {
            get { return "Full NEAT"; }
        }

        #endregion

        #region Experiment

        /// <summary>
        /// Gets and set the experiment for testing
        /// </summary>
        public IExperiment Experiment
        {
            get;
            set;
        }

        #endregion

        #region Generation

        /// <summary>
        /// Gets the number of current generation
        /// </summary>
        public int Generation 
        {
            get { return _generation; }
        }

        #endregion

        #region NEAT Parameters

        /// <summary>
        /// Provides a getter for the reference to the class of parameters 
        /// specific to NEAT
        /// </summary>
        public NEATParameters NEATParameters
        {
            get { return _neatParameters; }
        }

        #endregion

        #region General parameters

        /// <summary>
        /// Provides a getter for the reference to the class of general
        /// evolutionary algorithm parameters
        /// </summary>
        public GeneralEAParameters GeneralEAParameters
        {
            get { return _generalParameters; }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor initailizes the collections and sets the parameters
        /// </summary>
        public NEATAlgorithm()
        {
            _innovations = new List<Innovation>();
            _species = new List<Species>();
            _population = new Misc.SortedList<Transducer>();

            _neatParameters = new NEATParameters();
            _neatParameters.SurvivalRate = 0.5;
            _neatParameters.CompatibilityThreshold = 10;
            _neatParameters.MaxRelativeSpecieCount = 35;
            _neatParameters.AddNodeMutationProbability = 0.2;
            _neatParameters.AddTransitionMutationProbability = 0.85;
            _neatParameters.InnovationResetPerGeneration = true;

            _generalParameters = new GeneralEAParameters();
            _generalParameters.MutationProportion = 0d;
            _generalParameters.MaxIndividualSize = 3;
            _generalParameters.TransitionTriggerMutationProbability = 0.95;
            _generalParameters.TransitionActionMutationProbability = 0.88;
            _generalParameters.TransitionDeletionMutationProbability = 0.3;
            _generalParameters.StateDeletionMutationProbability = 0.45;
            _generalParameters.SelectionProportion = 0.65;

            random = new Random();
        }

        #endregion

        #region Private/protected methods

        #region Utility functions

        /// <summary>
        /// Creates a random transition with given innovation number
        /// </summary>
        private TransducerTransition CreateRandomTransition(int innovation)
        {
            return new TransducerTransition(
                Experiment.TransitionActions.ElementAt(random.Next(Experiment.TransitionActions.Count())),
                Experiment.TransitionTranslations.ElementAt(random.Next(Experiment.TransitionTranslations.Count())),
                innovation
            );
        }

        /// <summary>
        /// Creates a random transtion trigger - if possible with random conditional
        /// </summary>
        private TransitionTrigger CreateRandomTrigger()
        {
            var trigger = Experiment.TransitionEvents.ElementAt(random.Next(Experiment.TransitionEvents.Count()));
            if (trigger.IsConditional)
            {
                trigger.Parameter = random.Next((int)trigger.MinParameterValue, 
                    (int)trigger.MaxParameterValue) + random.NextDouble();
                var cond = random.Next(Enum.GetValues(typeof(TriggerConditionOperator)).Length);
                trigger.ConditionOperator = (TriggerConditionOperator)cond;
            }
            return trigger;
        }

        #endregion

        #region Innovation control

        /// <summary>
        /// Given the id of two states, detects whether a connection has added between them or not
        /// </summary>
        /// <returns>if the innovation exists it's number is returned, otherwise a global counter is 
        /// incremented and returned</returns>
        private int DetectInnovationNumberForConnection(int firstId, int secondId)
        {
            foreach (var innovation in _innovations)
            {
                if (innovation.FirstState == firstId && innovation.SecondState == secondId)
                {
                    return innovation.InnovationNumber;
                }
            }
            _innovations.Add(new Innovation(firstId, secondId, ++_globalInnovationNumber));
            return _globalInnovationNumber;
        }

        /// <summary>
        /// Given the id of two states, detects whether a node has been added between them or not
        /// </summary>
        /// <returns>a tuple of integers is returned in this order: innovation number of the connection 
        /// between the first and the new one, innovation number between the new and the second and the id of the new state</returns>
        private Tuple<int, int, int> DetectInnovationNumberForNodeMutation(int firstId, int secondId)
        {
            var firstCandidates = new Dictionary<int, int>();
            var secondCandidates = new Dictionary<int, int>();
            foreach (var innovation in _innovations)
            {
                if (innovation.FirstState == firstId && innovation.CreatedState != -1)
                {
                    firstCandidates.Add(innovation.SecondState, innovation.InnovationNumber);
                }
                if (innovation.SecondState == secondId && innovation.CreatedState != -1)
                {
                    secondCandidates.Add(innovation.FirstState, innovation.InnovationNumber);
                }
            }

            var intersection = firstCandidates.Keys.Intersect(secondCandidates.Keys);
            switch (intersection.Count())
            {
                case 0:
                    // innovation is new - a new state id and innovation numbers are assigned and the innovation is saved
                    _stateIndex++;
                    _globalInnovationNumber += 2;
                    _innovations.Add(new Innovation(firstId, _stateIndex, _globalInnovationNumber - 1, _stateIndex));
                    _innovations.Add(new Innovation(_stateIndex, secondId, _globalInnovationNumber, _stateIndex));
                    return new Tuple<int, int, int>(_globalInnovationNumber - 1, _globalInnovationNumber, _stateIndex);
                case 1:
                    // innovation already exists
                    var elem = intersection.ElementAt(0);
                    return new Tuple<int, int, int>(firstCandidates[elem], secondCandidates[elem], elem);
                default:
                    throw new ApplicationException("The same modification already occurred more than once. This should never happen!");
            }
        }

        #endregion

        #region Mutation

        /// <summary>
        /// Created a new transition by performing a structural mutation on an existing transducer
        /// </summary>
        /// <returns>the mutated transducer</returns>
        protected virtual Transducer MutateTransducer(Transducer transducer)                  
        {
            // 'Add connection' mutation - adds randomly a new transition between two states
            Action<Transducer> AddConnection = (T) =>
            {
                var first = random.Next(T.States.Count);
                var second = random.Next(T.States.Count);

                var attempts = 5;
                while (attempts-- > 0)
                {
                    if (first != second) break;
                    second = random.Next(T.States.Count);
                }
                if (first == second) return;

                var transition = CreateRandomTransition(DetectInnovationNumberForConnection
                    (T.States[first].ID, T.States[second].ID));
                T.AddTransition(T.States[first], T.States[second], 
                    CreateRandomTrigger(), transition);
            };

            // 'Add node' mutation - adds randomly a new state between two connected states 
            Action<Transducer, List<Tuple<int, int>>> AddNode = (T, Candidates) =>
            {
                if (Candidates.Count == 0) return;

                var selected = Candidates[random.Next(Candidates.Count)];
                var innovation = DetectInnovationNumberForNodeMutation(T.States[selected.Item1].ID, T.States[selected.Item2].ID);

                var firstTransition = CreateRandomTransition(innovation.Item1);
                var secondTransition = CreateRandomTransition(innovation.Item2);

                T.States[selected.Item1].RemoveTransition(T.States[selected.Item2].ID);
                var newState = new TransducerState(innovation.Item3);
                T.AddState(newState);
                T.AddTransition(T.States[selected.Item1], newState,
                    CreateRandomTrigger(), firstTransition);
                T.AddTransition(newState, T.States[selected.Item2],
                    CreateRandomTrigger(), secondTransition);
            };


            var mutated = (Transducer)transducer.Clone();

            var connected = new List<Tuple<int,int>>();

            for (int i = 0; i < mutated.States.Count; ++i)
            {
                for (int j = 0; j < mutated.States.Count; ++j)
                {
                    if (i == j) continue;
                    if (mutated.States[i].GetTransition(mutated.States[j].ID) != null)
                    {
                        connected.Add(new Tuple<int, int>(i, j));
                    }
                }
            }

            if (random.NextDouble() <= _neatParameters.AddTransitionMutationProbability)
            {
                AddConnection(mutated);
            }
            if (mutated.States.Count < _generalParameters.MaxIndividualSize
                && random.NextDouble() <= _neatParameters.AddNodeMutationProbability)
            {
                AddNode(mutated, connected);
            }

            return mutated;
        }

        /// <summary>
        /// Performs mutation by deletion of a node / transition
        /// </summary>
        /// <param name="t">the transducer for mutation</param>
        protected virtual void MutateByDeletion(Transducer t)
        {
            // remove transition mutation
            if (random.NextDouble() <= _generalParameters.TransitionDeletionMutationProbability)
            {
                var connected = new List<Tuple<int, int>>();
                for (int i = 0; i < t.States.Count; ++i)
                {
                    for (int j = 0; j < t.States.Count; ++j)
                    {
                        if (i == j) continue;
                        if (t.States[i].GetTransition(t.States[j].ID) != null)
                        {
                            connected.Add(new Tuple<int, int>(i, j));
                        }
                    }
                }
                if (connected.Count > 0)
                {
                    var selected = connected[random.Next(connected.Count)];
                    t.States[selected.Item1].RemoveTransition(t.States[selected.Item2].ID);
                }
            }

            // remove state mutation
            if (t.States.Count <= 1) return;
            if (random.NextDouble() <= _generalParameters.StateDeletionMutationProbability)
            {
                var index = random.Next(1, t.States.Count); // starting state cannot be deleted
                var selected = t.States[index];
                selected.ResetTransitions();
                t.RemoveStateAt(index);
                foreach (var s in t.States)
                {
                    var removed = s.RemoveTransition(selected.ID);
                    while (removed)
                    {
                        removed = s.RemoveTransition(selected.ID);
                    }
                }
            }
        }

        /// <summary>
        /// Performs mutation on a random transition
        /// </summary>
        /// <param name="t">the transducer for mutation</param>
        protected virtual void MutateTransition(Transducer t)
        {
            if (t.States.Count == 0) return;
            var selectedState = t.States[random.Next(t.States.Count)];
            var transitions = selectedState.GetListOfTransitions();
            if (transitions.Count == 0) return;

            var selected = transitions[random.Next(transitions.Count)];

            // trigger mutation
            if (random.NextDouble() <= _generalParameters.TransitionTriggerMutationProbability)
            {
                var trigger = CreateRandomTrigger();

                var dest = selectedState.GetDestinationIdByTrigger(selected.TransitionTrigger.TransitionEvent);
                selectedState.RemoveTransition(selected.TransitionTrigger.TransitionEvent);
                selectedState.AddTransition(trigger.TransitionEvent, selected, dest);
                selected.TransitionTrigger = trigger;
            }

            // action mutation
            if (random.NextDouble() <= _generalParameters.TransitionActionMutationProbability)
            {
                var action = Experiment.TransitionActions.ElementAt(random.Next(Experiment.TransitionActions.Count()));
                selected.TransitionAction = action;
            }
        }

        #endregion

        #region Crossover

        /// <summary>
        /// Performs a crossover of two transducers
        /// </summary>
        /// <param name="mommy">first parent</param>
        /// <param name="daddy">second parent</param>
        /// <returns>the resulting offspring</returns>
        protected virtual Transducer CrossTransducers(Transducer mommy, Transducer daddy)        
        {
            Action<Transducer, SortedDictionary<int, List<TransducerTransition>>> SaveTransitions = (T, D) =>
            {
                foreach (var state in T.States)
                {
                    var trans = state.GetListOfTransitions();
                    foreach (var tran in trans)
                    {
                        if (!D.ContainsKey(tran.InnovationNumber))
                        {
                            D[tran.InnovationNumber] = new List<TransducerTransition>();
                        }
                        D[tran.InnovationNumber].Add(tran);
                    }
                }
            };

            var transitions = new SortedDictionary<int, List<TransducerTransition>>();
            
            SaveTransitions(mommy, transitions);
            SaveTransitions(daddy, transitions);

            var offspring = new Transducer();

            foreach (var tran in transitions)
            {
                if (tran.Value.Count == 0) continue;

                var selected = tran.Value[random.Next(tran.Value.Count)];
                var from = new TransducerState(selected.StateFrom);
                var to = new TransducerState(selected.StateTo);
                offspring.AddTransition(from, to, selected.TransitionTrigger, (TransducerTransition)selected.Clone());
            }

            return offspring;
        }

        #endregion

        #region Speciation

        /// <summary>
        /// Inserts every transducer into a fitting species.
        /// If no suitable species exists a new one is created 
        /// </summary>
        protected virtual void SpeciatePopulation()
        {
            foreach (var individual in _population)
            {
                var foundSpecies = false;

                foreach (var species in _species)
                {
                    if (species.InsertNew(individual))
                    {
                        foundSpecies = true;
                        break;
                    }
                }
                if (!foundSpecies)
                {
                    var newSpecies = new Species(_speciesIndex++, individual, _neatParameters, _generalParameters);
                    _species.Add(newSpecies);
                }
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the population
        /// </summary>
        protected virtual void InitializePopulation()
        {
            // Starting out minimally - ???? only two states wit a random transition ????
            // TODO refactor this
            for (int i = 0; i < GeneralEAParameters.InitialPopulationSize; ++i)
            {
                var t = new Transducer();
                var s1 = new TransducerState(1);
                var s2 = new TransducerState(2);
                t.AddState(s1);
                t.AddState(s2);
                var trans = CreateRandomTransition(1);
                t.AddTransition(s1, s2,
                    CreateRandomTrigger(), trans);
                _population.Add(t);
            }

            _stateIndex = 2;
            _globalInnovationNumber++;
        }

        #endregion

        #endregion

        #region Public methods

        public Transducer Evolve()
        {
            // TODO
            // create a new thread for the evolutionary cycle so the gui stays responsive

            Logger.CurrentLogger.LogEvolutionStart(this.Name, Experiment.Name);

            InitializePopulation();
            // evaluate the initial population
            foreach (var t in _population)
            {
                var fitness = Experiment.Run(t);
                t.EvaluationInfo.Fitness = fitness;
            }

            // evolutionary cycle
            for (;;)
            {
                ++_generation;

                Logger.CurrentLogger.LogStat("Generation  ", _generation, "\r\n");
                Logger.CurrentLogger.LogStat("Population count", _population.Count);
                Logger.CurrentLogger.LogStat("Specie count", _species.Count);

                // check if there is a sufficient solution
                var bestOne = _population.Last();
                Logger.CurrentLogger.LogStat("Best fitness", bestOne.EvaluationInfo.Fitness);
                if (bestOne.EvaluationInfo.Fitness >= Experiment.RequiredFitness)
                {
                    Logger.CurrentLogger.LogEvolutionEnd(_generation, bestOne.ToString(), true);
                    return bestOne;
                }

                if (_generation == _generalParameters.GenerationThreshold)
                {
                    // no more generations allowed, return the most fit transducer
                    Logger.CurrentLogger.LogEvolutionEnd(_generation, bestOne.ToString(), false);
                    return _population.Last();
                }

                // tell each species it's a new generation, purge stagnating species
                var toKill = new HashSet<Species>();
                foreach (var species in _species)
                {
                    species.GenerationStart();
                    if (species.StagnatedGenerations > _neatParameters.AllowedSpeciesStagnatedGenerationCount)
                    {
                        toKill.Add(species);
                    }
                }
                Logger.CurrentLogger.LogStat("Species killed", toKill.Count);
                foreach (var s in toKill)
                {
                    s.Clear();
                    _species.Remove(s);
                }

                // Respeciate the population
                SpeciatePopulation();
                // adjust compatibility threashold based on the number of species
                if (_neatParameters.MaxRelativeSpecieCount > 1) // otherwise this feature is turned off 
                {
                    const double delta = 0.5;
                    if (_species.Count < _neatParameters.MaxRelativeSpecieCount * 0.1)
                    {
                        // more species needed, try higher threshold
                        _neatParameters.CompatibilityThreshold += delta;
                    }
                    else if (_species.Count > _neatParameters.MaxRelativeSpecieCount)
                    {
                        // too many species, lower the threshold
                        _neatParameters.CompatibilityThreshold -= delta;
                    }
                }
                Logger.CurrentLogger.LogStat("Species threshold", _neatParameters.CompatibilityThreshold);

                // a collection of individuals, which will replace the current generation
                var newGeneration = new Misc.SortedList<Transducer>();

                // For each species perform selection and mutation resp. crossover
                foreach (var species in _species)
                {
                    int amountToSpawn = species.SpawnCount;
                    while (amountToSpawn-- > 0)
                    {
                        Transducer babyTransducer;

                        var c = species.GetCandidateForMultiplication();
                        if (species.Population.Count > 1 && random.NextDouble() > 0.4)
                        {
                            // crossover
                            var cc = species.GetCandidateForMultiplication();
                            int attemptsLeft = 5;
                            while (attemptsLeft-- > 0)
                            {
                                if (c != cc) break;
                                cc = species.GetCandidateForMultiplication();
                            }
                            
                            if (c == cc) continue;
                            babyTransducer = CrossTransducers(c, cc);
                        }
                        else
                        {
                            // structural mutation
                            babyTransducer = MutateTransducer(c);
                        }

                        // mutate the newcomer
                        babyTransducer = MutateTransducer(babyTransducer);
                        MutateTransition(babyTransducer);
                        MutateByDeletion(babyTransducer);
                        
                        // evaluate the newcomer
                        var fitness = Experiment.Run(babyTransducer);
                        babyTransducer.EvaluationInfo.Fitness = fitness;

                        newGeneration.Add(babyTransducer);
                    }
                }
                Logger.CurrentLogger.LogStat("Newbies created", newGeneration.Count);

                // perform mutation among the veterans - totally redundant in full neat
                var amount = Convert.ToInt32(_population.Count * _generalParameters.MutationProportion);
                while (amount-- > 0)
                {
                    var t = _population.ElementAt(random.Next(_population.Count));
                    var newOne = MutateTransducer(t);
                    MutateTransition(newOne);
                    MutateByDeletion(newOne);
                    // reevaluate it
                    var f = Experiment.Run(newOne);
                    newOne.EvaluationInfo.Fitness = f;
                    newGeneration.Add(newOne);
                }

                // force out explicit fitness sharing and clear the species
                foreach (var species in _species)
                {
                    species.CheckStagnation();
                    species.AdjustFitness();
                    species.SelectNewRepresentative();
                    species.Clear();
                }

                // replace population
                var amountToSurvive = Convert.ToInt32(_population.Count * _generalParameters.ReplacementProportion);
                if (amountToSurvive + newGeneration.Count > _generalParameters.MaxPopulationSize)
                {
                    amountToSurvive = _generalParameters.MaxPopulationSize - newGeneration.Count;
                }
                var survivors = _population.Select(x => x).Skip(_population.Count - amountToSurvive);
                newGeneration.AddRange(survivors);
                _population = newGeneration;

                Logger.CurrentLogger.LogStat("State id reached", _stateIndex);
                Logger.CurrentLogger.LogStat("Innovations found", _globalInnovationNumber);

                // reset innovations if specified
                if (_neatParameters.InnovationResetPerGeneration)
                {
                    _innovations.Clear();
                }
            }
        }

        #endregion

    }

}

