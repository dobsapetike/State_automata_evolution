using System;
using System.Collections.Generic;
using System.Linq;
using Misc = BenchmarkDepot.Classes.Misc;
using BenchmarkDepot.Classes.Misc;
using BenchmarkDepot.Classes.Core.Experiments;
using BenchmarkDepot.Classes.Core.EAlgotihms.Parameters;
using BenchmarkDepot.Classes.Core.EAlgotihms.Accessories;
using System.Threading;

namespace BenchmarkDepot.Classes.Core.EAlgotihms
{

    /// <summary>
    /// Full NEAT algorithm with all of it's features - also a base class for the other
    /// evolutionary algorithms, which are basically the same with some ablations
    /// </summary>
    public class NEATAlgorithm : ObservableObject
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

        /// <summary>
        /// Flag indicating whether the evolution is in process
        /// </summary>
        protected bool _isRunning;

        #endregion

        #region Parameters

        protected NEATParameters _neatParameters;
        protected GeneralEAParameters _generalParameters;

        #endregion

        /// <summary>
        /// List of all innovations that occurred during the current generation
        /// </summary>
        protected Dictionary<int, List<Innovation>> _innovations;

        /// <summary>
        /// List of all species in the population
        /// </summary>
        protected List<Species> _species;

        /// <summary>
        /// Sorted collection of all individuals in the population
        /// </summary>
        protected Misc.SortedList<Transducer> _population;

        private Random random;

        private List<Tuple<string, Action<Transducer>>> _listOfMutations;

        private Dictionary<string, List<double>> _mutationSuccess;

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

        #region Population count

        /// <summary>
        /// Gets the number of individuals
        /// </summary>
        public int PopulationCount
        {
            get { return _population.Count; }
        }

        #endregion

        #region Specie count

        /// <summary>
        /// Gets the number of species
        /// </summary>
        public int SpecieCount
        {
            get { return _species.Count; }
        }

        #endregion

        #region BestFitness

        /// <summary>
        /// Gets the fitness of the best individual
        /// </summary>
        public double BestFitness
        {
            get { return _bestFitness; }
            private set
            {
                _bestFitness = value;
                RaisePropertyChanged(() => BestFitness);
            }
        }

        private double _bestFitness;

        #endregion

        #region Evaluation count

        /// <summary>
        /// Gets the total number of evaluations
        /// </summary>
        public long EvaluationCount
        {
            get;
            private set;
        }

        #endregion

        #region State id

        /// <summary>
        /// Gets the current state id index
        /// </summary>
        public int StateIdCount
        {
            get { return _stateIndex; }
        }

        #endregion

        #region Innovation count

        /// <summary>
        /// Gets the current innovation index
        /// </summary>
        public int InnovationCount
        {
            get { return _globalInnovationNumber; }
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

        #region Evolution result

        /// <summary>
        /// The reulting transducer of the last evolution
        /// </summary>
        public Transducer EvolutionResult
        {
            get { return _evolResult; }
            private set
            {
                _evolResult = value;
                RaisePropertyChanged(() => EvolutionResult);
            }
        }

        private Transducer _evolResult = null;

        #endregion

        #endregion

        #region Event

        #region Alert event

        public delegate void AlertEventHandler(object sender, AlertEventArgs args);

        /// <summary>
        /// Raised every time the algorithm sends an alert message
        /// </summary>
        public event AlertEventHandler AlertEvent;

        protected virtual void RaiseAlertEvent(string alert)
        {
            if (AlertEvent == null) return;
            AlertEvent(this, new AlertEventArgs(alert));
        }

        #endregion

        #region Generation end evet

        public delegate void GenerationEndHandler(object sender, GenerationEndArgs args);

        /// <summary>
        /// Raised at the end of each generation
        /// </summary>
        public event GenerationEndHandler GenerationEndEvent;

        protected virtual void RaiseGenerationEndEvent()
        {
            if (GenerationEndEvent == null) return;
            GenerationEndEvent(this, new GenerationEndArgs 
            { 
                Generation = _generation, 
                BestFitness = _bestFitness, 
                EvaliationCount = this.EvaluationCount
            }); 
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor initailizes the collections and sets the parameters
        /// </summary>
        public NEATAlgorithm()
        {
            _innovations = new Dictionary<int, List<Innovation>>();
            _species = new List<Species>();
            _population = new Misc.SortedList<Transducer>();
            _mutationSuccess = new Dictionary<string, List<double>>();

            var paramPreset = new DefaultNeatPreset();
            _neatParameters = new NEATParameters();
            _neatParameters.LoadFromPreset(paramPreset);

            _generalParameters = new GeneralEAParameters();
            _generalParameters.LoadFromPreset(paramPreset);

            random = new Random();
        }

        #endregion

        #region Private/protected methods

        #region Utility functions

        /// <summary>
        /// Creates a random transition with given innovation number
        /// </summary>
        protected TransducerTransition CreateRandomTransition(int innovation)
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
        protected TransitionTrigger CreateRandomTrigger()
        {
            var trigger = Experiment.TransitionEvents.ElementAt(random.Next(Experiment.TransitionEvents.Count()));
            if (trigger.IsConditional)
            {
                trigger.Parameter1 = random.Next((int)trigger.MinParameterValue, 
                    (int)trigger.MaxParameterValue);
                trigger.Parameter2 = random.Next((int)trigger.Parameter1,
                    (int)trigger.MaxParameterValue);
                var cond = random.Next(Enum.GetValues(typeof(TriggerConditionOperator)).Length);
                trigger.ConditionOperator = (TriggerConditionOperator)cond;
            }
            return trigger;
        }

        /// <summary>
        /// Resets collections and fields
        /// </summary>
        public void Reset()
        {
            if (_isRunning) return;

            _innovations.Clear();
            foreach (var species in _species)
            {
                species.Clear();
            }
            _species.Clear();
            _population.Clear();

            _generation = 0;
            _speciesIndex = 0;
            _stateIndex = 0;
            EvaluationCount = 0;
            _globalInnovationNumber = 0;
            EvolutionResult = null;

            _mutationSuccess.Clear();

            StatNotification();
        }

        #endregion

        #region Innovation control

        /// <summary>
        /// Given the id of two states, detects whether a connection has added between them or not
        /// </summary>
        /// <returns>if the innovation exists it's number is returned, otherwise a global counter is 
        /// incremented and returned</returns>
        protected int DetectInnovationNumberForConnection(int firstId, int secondId)
        {
            if (!_innovations.ContainsKey(firstId))
            {
                _innovations[firstId] = new List<Innovation>();
            }

            foreach (var innovation in _innovations[firstId])//_innovations)
            {
                if (innovation.FirstState == firstId && innovation.SecondState == secondId)
                {
                    return innovation.InnovationNumber;
                }
            }
            _innovations[firstId].Add(new Innovation(firstId, secondId, ++_globalInnovationNumber));
            return _globalInnovationNumber;
        }

        /// <summary>
        /// Given the id of two states, detects whether a node has been added between them or not
        /// </summary>
        /// <returns>a tuple of integers is returned in this order: innovation number of the connection 
        /// between the first and the new one, innovation number between the new and the second and the id of the new state</returns>
        protected Tuple<int, int, int> DetectInnovationNumberForNodeMutation(int firstId, int secondId)
        {
            if (!_innovations.ContainsKey(firstId))
            {
                _innovations[firstId] = new List<Innovation>();
            }

            var firstCandidates = new Dictionary<int, int>();
            var secondCandidates = new Dictionary<int, int>();
            foreach (var innovation in _innovations[firstId])
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
                    _innovations[firstId].Add(new Innovation(firstId, _stateIndex, _globalInnovationNumber-1, _stateIndex));
                    _innovations[firstId].Add(new Innovation(_stateIndex, secondId, _globalInnovationNumber, _stateIndex));
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
        /// Adds a random trandom transition to the transducer
        /// </summary>
        private void AddTransitionMutation(Transducer t)
        {
            if (t.States.Count == 0) return;
            if (random.NextDouble() > _neatParameters.AddTransitionMutationProbability) return;

            var first = random.Next(t.States.Count);
            var second = random.Next(t.States.Count);

            var transition = CreateRandomTransition(DetectInnovationNumberForConnection
                (t.States[first].ID, t.States[second].ID));
            t.AddTransition(t.States[first], t.States[second],
                CreateRandomTrigger(), transition);
        }

        /// <summary>
        /// Adds randomly a new state between two connected states 
        /// </summary>
        private void AddNodeMutation(Transducer t)
        {
            if (t.States.Count == 0 || t.States.Count >= _generalParameters.MaxIndividualSize) return;
            if (random.NextDouble() > _neatParameters.AddNodeMutationProbability) return;

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

            if (connected.Count == 0) return;

            var selected = connected[random.Next(connected.Count)];
            var innovation = DetectInnovationNumberForNodeMutation(t.States[selected.Item1].ID, 
                t.States[selected.Item2].ID);

            var firstTransition = CreateRandomTransition(innovation.Item1);
            var secondTransition = CreateRandomTransition(innovation.Item2);

            t.States[selected.Item1].RemoveTransition(t.States[selected.Item2].ID);
            var newState = new TransducerState(innovation.Item3);
            t.AddState(newState);
            t.AddTransition(t.States[selected.Item1], newState,
                CreateRandomTrigger(), firstTransition);
            t.AddTransition(newState, t.States[selected.Item2],
                CreateRandomTrigger(), secondTransition);
        }

        /// <summary>
        /// Removes a random transition from the transducer
        /// </summary>
        private void RemoveTransitionMutation(Transducer t)
        {
            if (random.NextDouble() > _generalParameters.TransitionDeletionMutationProbability) return;

            var connected = new List<Tuple<int, int>>();
            for (int i = 0; i < t.States.Count; ++i)
            {
                for (int j = 0; j < t.States.Count; ++j)
                {
                    if (t.States[i].GetTransition(t.States[j].ID) != null)
                    {
                        connected.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
            if (connected.Count > 0)
            {
                var selected = connected[random.Next(connected.Count)];
                if (t.States[selected.Item1].GetListOfTransitions().Count <= 1) return;
                t.States[selected.Item1].RemoveTransition(t.States[selected.Item2].ID);
            }
        }

        /// <summary>
        /// Removes a random state from the transducer
        /// </summary>
        private void RemoveStateMutation(Transducer t)
        {
            if (t.States.Count <= 2) return;
            if (random.NextDouble() > _generalParameters.StateDeletionMutationProbability) return;

            var transBuff = t.Clone();
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
        
        /// <summary>
        /// Performs a trigger mutation on the transducer
        /// </summary>
        private void MutateRandomTrigger(Transducer t)
        {
            if (t.States.Count == 0) return;
            if (random.NextDouble() > _generalParameters.TransitionTriggerMutationProbability) return;

            var selectedState = t.States[random.Next(t.States.Count)];
            var transitions = selectedState.GetListOfTransitions();
            if (transitions.Count == 0) return;
            
            var selected = transitions[random.Next(transitions.Count)];
            var trigger = CreateRandomTrigger();
            var dest = selectedState.GetDestinationIdByTrigger(selected.TransitionTrigger.TransitionEvent);
            selectedState.RemoveTransition(selected.TransitionTrigger.TransitionEvent);
            selectedState.AddTransition(trigger.TransitionEvent, selected, dest);
            selected.TransitionTrigger = trigger;
        }

        /// <summary>
        /// Performs an action mutation on the transducer
        /// </summary>
        private void MutateRandomAction(Transducer t)
        {
            if (t.States.Count == 0) return;
            if (random.NextDouble() > _generalParameters.TransitionActionMutationProbability) return;

            var selectedState = t.States[random.Next(t.States.Count)];
            var transitions = selectedState.GetListOfTransitions();
            if (transitions.Count == 0) return;

            var selected = transitions[random.Next(transitions.Count)];
            var action = Experiment.TransitionActions.
                    ElementAt(random.Next(Experiment.TransitionActions.Count()));
            selected.TransitionAction = action;
        }

        /// <summary>
        /// Performs a translation mutation on the transducer
        /// </summary>
        private void MutateRandomTranslation(Transducer t)
        {
            if (t.States.Count == 0) return;
            if (random.NextDouble() > _generalParameters.TransitionTranslationMutationProbability) return;

            var selectedState = t.States[random.Next(t.States.Count)];
            var transitions = selectedState.GetListOfTransitions();
            if (transitions.Count == 0) return;

            var selected = transitions[random.Next(transitions.Count)];
            var translation = Experiment.TransitionTranslations
                    .ElementAt(random.Next(Experiment.TransitionTranslations.Count()));
            selected.Translation = translation;
        }

        /// <summary>
        /// Initializes the list of every available mutations
        /// </summary>
        private void ConstructListOfMutations()
        {
            _listOfMutations = new List<Tuple<string, Action<Transducer>>>();

            // if there is no chance, don't put amongst the options
            if (_neatParameters.AddNodeMutationProbability != 0d) 
                _listOfMutations.Add(new Tuple<string, Action<Transducer>>("Add node", AddNodeMutation));
            if (_neatParameters.AddTransitionMutationProbability != 0d) 
                _listOfMutations.Add(new Tuple<string, Action<Transducer>>("Add transition", AddTransitionMutation));
            if (_generalParameters.TransitionDeletionMutationProbability != 0d)
                _listOfMutations.Add(new Tuple<string, Action<Transducer>>("Delete transition", RemoveTransitionMutation));
            if (_generalParameters.StateDeletionMutationProbability != 0d)
                _listOfMutations.Add(new Tuple<string, Action<Transducer>>("Delete node", RemoveStateMutation));
            if (_generalParameters.TransitionTriggerMutationProbability != 0d) 
                _listOfMutations.Add(new Tuple<string, Action<Transducer>>("Mutate trigger", MutateRandomTrigger));
            if (_generalParameters.TransitionActionMutationProbability != 0d) 
                _listOfMutations.Add(new Tuple<string, Action<Transducer>>("Mutate action", MutateRandomAction));
            if (_generalParameters.TransitionTranslationMutationProbability != 0d) 
                _listOfMutations.Add(new Tuple<string, Action<Transducer>>("Mutate translation", MutateRandomTranslation));
        }

        /// <summary>
        /// Performs mutation on a random transition
        /// </summary>
        protected virtual void MutateTransducer(Transducer t)
        {
            if (_listOfMutations.Count == 0) return;
            // choose one and use it
            var selectedMutation = _listOfMutations[random.Next(_listOfMutations.Count)];
            selectedMutation.Item2.Invoke(t);

            t.EvaluationInfo.LastMutation = selectedMutation.Item1; // also save the mutation name
        }

        #endregion

        #region Crossover

        /// <summary>
        /// Performs a crossover of two transducers
        /// </summary>
        /// <param name="mommy">first parent</param>
        /// <param name="daddy">second parent</param>
        /// <returns>the resulting offspring</returns>
        public virtual Transducer CrossTransducers(Transducer mommy, Transducer daddy)        
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

            var addedIds = new HashSet<int>();
            foreach (var tran in transitions)
            {
                if (tran.Value.Count == 0) continue;

                var selected = tran.Value[random.Next(tran.Value.Count)];
                // size control, so the offspring won't be bigger than the max size
                if (!addedIds.Contains(selected.StateFrom))
                {
                    if (addedIds.Count == _generalParameters.MaxIndividualSize) continue;
                    addedIds.Add(selected.StateFrom);
                }
                if (!addedIds.Contains(selected.StateTo))
                {
                    if (addedIds.Count == _generalParameters.MaxIndividualSize) continue;
                    addedIds.Add(selected.StateTo);
                }
                var from = new TransducerState(selected.StateFrom);
                var to = new TransducerState(selected.StateTo);
                offspring.AddTransition(from, to, selected.TransitionTrigger, selected.Clone());
            }
            return offspring;
        }

        #endregion

        #region Speciation

        /// <summary>
        /// Inserts every transducer into a fitting species.
        /// If no suitable species exists a new one is created 
        /// </summary>
        protected virtual void SpeciatePopulationAndCalculateSpawnCount()
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
            // now that species are created set the spawn count
            var avrgCount = (int)((_generalParameters.MaxPopulationSize * 
                _generalParameters.ReplacementProportion) / _species.Count);
            foreach (var s in _species) s.SpawnCount = avrgCount;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the population
        /// </summary>
        protected virtual void InitializePopulation()
        {
            // Starting out minimally - only two states wit a random transition
            for (int i = 0; i < GeneralEAParameters.InitialPopulationSize; ++i)
            {
                var t = new Transducer();
                var s1 = new TransducerState(1);
                t.AddState(s1);
                if (_generalParameters.MaxIndividualSize > 1)
                {
                    var s2 = new TransducerState(2);
                    t.AddState(s2);
                    var trans = CreateRandomTransition(DetectInnovationNumberForConnection(s1.ID, s2.ID));
                    t.AddTransition(s1, s2, CreateRandomTrigger(), trans);
                }
                _population.Add(t);
            }

            _stateIndex = 2;
        }

        #endregion

        #region Stat notification

        /// <summary>
        /// Notifies observers that stats have been updated
        /// </summary>
        private void StatNotification()
        {
            RaisePropertyChanged(() => Generation);
            RaisePropertyChanged(() => PopulationCount);
            RaisePropertyChanged(() => SpecieCount);
            RaisePropertyChanged(() => StateIdCount);
            RaisePropertyChanged(() => InnovationCount);
            RaisePropertyChanged(() => EvaluationCount);
        }

        #endregion

        #region Evolutionary Cycle

        private void OnEvolutionEnd(bool success, Transducer best)
        {
            Logger.CurrentLogger.LogEvolutionEnd(_generation, best.ToString(), success);
            RaiseGenerationEndEvent();
            StatNotification();
            EvolutionResult = best;
            _isRunning = false;
        }

        private void PerformEvolution()
        {
            RaiseAlertEvent("Evolution started!");
            Logger.CurrentLogger.LogEvolutionStart(this.Name, Experiment.Name);

            Reset();
            ConstructListOfMutations();
            _isRunning = true;

            InitializePopulation();
            // evaluate the initial population
            foreach (var t in _population)
            {
                var fitness = Experiment.Run(t);
                t.EvaluationInfo.Fitness = fitness;
                EvaluationCount++;
            }

            // evolutionary cycle
            for (;;)
            {
                // check if there is a sufficient solution
                var bestOne = _population.MaxBy(x => x.EvaluationInfo.Fitness);
                BestFitness = bestOne.EvaluationInfo.Fitness;

                if (_isRunning == false)
                {
                    // someone requested to abort the process
                    StatNotification();
                    RaiseAlertEvent("Evolution ended!\nStatus: unsuccessful, process aborted\n" + bestOne);
                    EvolutionResult = null;
                    return;
                }

                ++_generation;

                Logger.CurrentLogger.LogStat("Generation  ", _generation, "\r\n");
                Logger.CurrentLogger.LogStat("Population count", _population.Count);
                Logger.CurrentLogger.LogStat("Specie count", _species.Count);

                Logger.CurrentLogger.LogGraphData(bestOne.EvaluationInfo.Fitness.ToString());
                Logger.CurrentLogger.LogStat("Best fitness", bestOne.EvaluationInfo.Fitness);
                if (bestOne.EvaluationInfo.Fitness >= Experiment.RequiredFitness)
                {
                    RaiseAlertEvent("Evolution ended!\nStatus: successful, solution found\nResult:\n" + bestOne.ToString());
                    //foreach (var mutation in _mutationSuccess)
                    //{
                    //    Logger.CurrentLogger.LogStat(mutation.Key, mutation.Value.Average());
                    //}
                    OnEvolutionEnd(true, bestOne);
                    return;
                }

                if (_generation == _generalParameters.GenerationThreshold)
                {
                    // no more generations allowed
                    RaiseAlertEvent("Evolution ended!\nStatus: unsuccessful, generation threshold reached");
                    OnEvolutionEnd(false, bestOne);                    
                    return;
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
                SpeciatePopulationAndCalculateSpawnCount();
                // adjust compatibility threashold based on the number of species
                if (_neatParameters.CriticalSpecieCount > 1) // otherwise this feature is turned off 
                {
                    if (_species.Count < _neatParameters.CriticalSpecieCount * 0.25)
                    {
                        // more species needed, try higher threshold
                        _neatParameters.CompatibilityThreshold -= _neatParameters.CompatibilityThresholdDelta;
                    }
                    else if (_species.Count > _neatParameters.CriticalSpecieCount)
                    {
                        // too many species, lower the threshold
                        _neatParameters.CompatibilityThreshold += _neatParameters.CompatibilityThresholdDelta;
                    }
                }
                Logger.CurrentLogger.LogStat("Species threshold", _neatParameters.CompatibilityThreshold);

                // a collection of individuals, which will replace the current generation
                var newGeneration = new Misc.SortedList<Transducer>();

                // for each species perform selection and mutation resp. crossover
                foreach (var species in _species)
                {
                    bool isBestIn = !_generalParameters.Elitism; // if elitism is on, best is not there yet
                    int amountToSpawn = species.SpawnCount;
                    while (amountToSpawn-- > 0)
                    {
                        Transducer babyTransducer;

                        if (!isBestIn)
                        {
                            // the best of the species should always survive
                            isBestIn = true;
                            babyTransducer = species.Population.MaxBy(x => x.EvaluationInfo.Fitness).Clone();
                        }
                        else
                        {
                            var c = species.GetCandidateForMultiplication();
                            if (species.Population.Count > 1 && random.NextDouble() <= _generalParameters.CrossoverProbability)
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
                                babyTransducer = c.Clone();
                            }

                            // check out fitness of the baby before any mutation
                            var fitnessBefore = Experiment.Run(babyTransducer);
                            // mutate the newcomer
                            for (var i = 0; i < _generalParameters.MutationCount; ++i)
                            {
                                var cache = babyTransducer.Clone();
                                MutateTransducer(babyTransducer);
                                if (babyTransducer.GetTransitionCount() == 0) babyTransducer = cache;
                            }
                            // evaluate the newcomer
                            var fitness = Experiment.Run(babyTransducer);
                            babyTransducer.EvaluationInfo.Fitness = fitness;
                            EvaluationCount++;
                            // let's see how good was the mutation
                            var mutation = babyTransducer.EvaluationInfo.LastMutation;
                            if (mutation != null)
                            {
                                if (!_mutationSuccess.ContainsKey(mutation))
                                {
                                    _mutationSuccess.Add(mutation, new List<double>());
                                }
                                if (fitness - fitnessBefore > 0) _mutationSuccess[mutation].Add(fitness - fitnessBefore);
                            }
                        }

                        newGeneration.Add(babyTransducer);
                    }
                }
                Logger.CurrentLogger.LogStat("Newbies created", newGeneration.Count);


                // force out explicit fitness sharing and clear the species
                foreach (var species in _species)
                {
                    species.CheckStagnation();
                    species.AdjustFitness();
                    species.SelectNewRepresentative();
                    species.Clear();
                }

                // replace population
                var amountToSurvive = _generalParameters.MaxPopulationSize - newGeneration.Count;
                if (amountToSurvive > 0)
                {
                    var survivors = _population.Select(x => x).Skip(_population.Count - amountToSurvive);
                    newGeneration.AddRange(survivors);
                }
                _population = newGeneration;

                Logger.CurrentLogger.LogStat("State id reached", _stateIndex);
                Logger.CurrentLogger.LogStat("Innovations found", _globalInnovationNumber);

                // reset innovations if specified
                if (_neatParameters.InnovationResetPerGeneration)
                {
                    _innovations.Clear();
                }

                // update observer stats
                StatNotification();
                RaiseGenerationEndEvent();
            }
        }

        #endregion

        #endregion

        #region Public methods

        /// <summary>
        /// Starts the evolutionary cycle on a dedicated thread
        /// </summary>
        public void Evolve(Action<IAsyncResult> callBack)
        {
            var evol = new Action(PerformEvolution);
            evol.BeginInvoke(new AsyncCallback(callBack), null);
        }

        /// <summary>
        /// Stops the evolutionary cycle after the actual iteration is fully executed.
        /// </summary>
        public void RequestStopEvolution()
        {
            if (_isRunning)
            {
                _isRunning = false;
            }
        }

        #endregion

    }

}

