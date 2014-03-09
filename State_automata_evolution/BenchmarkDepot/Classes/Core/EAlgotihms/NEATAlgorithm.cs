using System;
using System.Collections.Generic;
using System.Linq;
using Misc = BenchmarkDepot.Classes.Misc;
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
            _neatParameters.SurvivalRate = 0.35;
            _neatParameters.CompatibilityThreshold = 1.5;
            _neatParameters.MaxSpecieSize = 35;

            _generalParameters = new GeneralEAParameters();

            random = new Random();
        }

        #endregion

        #region Private/protected methods

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

        /// <summary>
        /// Inserts every transducer into a fitting species.
        /// If no suitable species exists a new one is created 
        /// </summary>
        /// <param name="lastGenerationSpecieCount">Number of species created in the 
        /// last generation for adjusting the species compatibility threshold</param>
        protected virtual void SpeciatePopulation(int lastGenerationSpecieCount)
        {
            foreach (var individual in _population)
            {
                var foundSpecies = false;

                while (!foundSpecies)
                {
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
                        if (_species.Count == _neatParameters.MaxSpecieSize)
                        {
                            // new species cannot be created, so lower the threshold and try again
                            _neatParameters.CompatibilityThreshold -= 0.15;
                            if (_neatParameters.CompatibilityThreshold <= _neatParameters.MinCompatibilityThreshold)
                            {
                                // no suitable species and the threshold can not be lowered - skip this transducer
                                foundSpecies = true;
                            }
                            continue;
                        }
                        foundSpecies = true;
                        var newSpecies = new Species(_speciesIndex++, individual, _neatParameters, _generalParameters);
                        _species.Add(newSpecies);
                    }
                } // found species
            }  // next transducer
        }

        // Creates a random transition with given innovation number
        private TransducerTransition CreateRandomTransition(int innovation)
        {
            return new TransducerTransition(
                Experiment.TransitionActions.ElementAt(random.Next(Experiment.TransitionActions.Count())),
                Experiment.TransitionTranslations.ElementAt(random.Next(Experiment.TransitionTranslations.Count())),
                innovation
            );
        }

        /// <summary>
        /// Initializes the population
        /// </summary>
        protected virtual void Initialization()
        {
            // Starting out minimally - ???? only two states without any connections ????
            for (int i = 0; i < GeneralEAParameters.InitialPopulationSize; ++i)
            {
                var t = new Transducer();
                var s1 = new TransducerState(1);
                var s2 = new TransducerState(2);
                t.AddState(s1);
                t.AddState(s2);
                var trans = CreateRandomTransition(1);
                t.AddTransition(s1, s2, 
                    Experiment.TransitionEvents.ElementAt(random.Next(Experiment.TransitionEvents.Count())), trans);
                _population.Add(t);
            }
            _stateIndex = 2;
            _globalInnovationNumber++;
        }

        /// <summary>
        /// Performs a mutation on a transducer
        /// </summary>
        /// <returns>the mutated transducer</returns>
        protected virtual Transducer MutateTransducer(Transducer transducer)                  
        {
            // 'Add connection' mutation - adds randomly a new transition between two states
            Action<Transducer, List<Tuple<int, int>>> AddConnection = (T,Candidates) =>
            {
                if (Candidates.Count == 0) return;

                var selected = Candidates[random.Next(Candidates.Count)];

                var transition = CreateRandomTransition(DetectInnovationNumberForConnection
                    (T.States[selected.Item1].ID, T.States[selected.Item2].ID));
                T.AddTransition(T.States[selected.Item1], T.States[selected.Item2],
                    Experiment.TransitionEvents.ElementAt(random.Next(Experiment.TransitionEvents.Count())), transition);
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
                    Experiment.TransitionEvents.ElementAt(random.Next(Experiment.TransitionEvents.Count())), firstTransition);
                T.AddTransition(newState, T.States[selected.Item2],
                    Experiment.TransitionEvents.ElementAt(random.Next(Experiment.TransitionEvents.Count())), secondTransition);
            };


            var mutated = (Transducer)transducer.Clone();

            List<Tuple<int, int>> notConnected = new List<Tuple<int, int>>(), 
                connected = new List<Tuple<int,int>>();

            for (int i = 0; i < mutated.States.Count; ++i)
            {
                for (int j = 0; j < mutated.States.Count; ++j)
                {
                    if (i == j) continue;
                    if (mutated.States[i].GetTransition(mutated.States[j].ID) == null)
                    {
                        notConnected.Add(new Tuple<int, int>(i, j));
                    }
                    else
                    {
                        connected.Add(new Tuple<int, int>(i, j));
                    }
                }
            }

            if (random.NextDouble() <= _neatParameters.AddTransitionMutationProbability)
            {
                AddConnection(mutated, notConnected);
            }
            if (random.NextDouble() <= _neatParameters.AddNodeMutationProbability)
            {
                AddNode(mutated, connected);
            }

            return mutated;
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
                var trigger = Experiment.TransitionEvents.ElementAt(random.Next(Experiment.TransitionEvents.Count()));

                var dest = selectedState.GetDestinationIdByTrigger(selected.TransitionEvent);
                selectedState.RemoveTransition(selected.TransitionEvent);
                selectedState.AddTransition(trigger, selected, dest);
                selected.TransitionEvent = trigger;
            }

            // action mutation
            if (random.NextDouble() <= _generalParameters.TransitionActionMutationProbability)
            {
                var action = Experiment.TransitionActions.ElementAt(random.Next(Experiment.TransitionActions.Count()));
                selected.TransitionAction = action;
            }
        }

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
                offspring.AddTransition(from, to, selected.TransitionEvent, (TransducerTransition)selected.Clone());
            }

            return offspring;
        }

        #endregion

        #region Public methods

        public Transducer Evolve()
        {
            // TODO
            // create a new thread for the evolutionary cycle so the gui stays responsive

            Initialization();
            // evaluate the initial population
            foreach (var t in _population)
            {
                var fitness = Experiment.Run(t);
                t.EvaluationInfo.Fitness = fitness;
            }

            var lastGenerationSpecieCount = _species.Count;

            // evolutionary cycle
            for (;;)
            {

                if (++_generation == _generalParameters.GenerationThreshold)
                {
                    // return the most fit transducer
                    return _population.Last();
                }

                // check if there is a sufficient solution
                var bestOne = _population.Last();
                if (bestOne.EvaluationInfo.Fitness >= Experiment.RequiredFitness)
                {
                    return bestOne;
                }

                var newGeneration = new Misc.SortedList<Transducer>();

                // Respeciate the population
                SpeciatePopulation(lastGenerationSpecieCount);
                if (lastGenerationSpecieCount == _species.Count)
                {
                    // no new species created, lower the threshold
                    _neatParameters.CompatibilityThreshold -= 0.15;
                }
                else if (_species.Count - lastGenerationSpecieCount >= _species.Count / 3)
                {
                    // many new species created, try higher threshold
                    _neatParameters.CompatibilityThreshold += 0.15;
                }
                lastGenerationSpecieCount = _species.Count;


                // For each species perform selection and mutation resp. crossover
                foreach (var species in _species)
                {
                    int amountToSpawn = species.SpawnCount;
                    while (amountToSpawn-- > 0)
                    {
                        Transducer babyTransducer;

                        var c = species.GetCandidateForMultiplication();
                        if (random.NextDouble() > 0.5)
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
                        var count = random.Next(10);
                        for (int i = 0; i < random.Next(10); ++i)
                        {
                            babyTransducer = MutateTransducer(babyTransducer);
                            MutateTransition(babyTransducer);
                        }
                        
                        // evaluate the newcomer
                        var fitness = Experiment.Run(babyTransducer);
                        babyTransducer.EvaluationInfo.Fitness = fitness;

                        newGeneration.Add(babyTransducer);
                    }
                }

                // also perform mutation among the veterans
                var amount = Convert.ToInt32(_population.Count * _generalParameters.MutationProportion);
                while (amount-- > 0)
                {
                    var t = _population.ElementAt(random.Next(_population.Count));
                    var count = random.Next(5);
                    for (int i = 0; i < count; ++i)
                    {
                        t = MutateTransducer(t);
                        MutateTransition(t);
                    }
                    // reevaluate it
                    var f = Experiment.Run(t);
                    t.EvaluationInfo.Fitness = f;
                }

                // force out explicit fitness sharing and clear the species
                foreach (var species in _species)
                {
                    species.AdjustFitness();
                    species.SelectNewRepresentative();
                    species.Clear();
                }

                // replace population
                var amountToSurvive = Convert.ToInt32(_population.Count * _generalParameters.ReplacementProportion);
                var survivors = _population.Select(x => x).Skip(_population.Count - amountToSurvive);
                newGeneration.AddRange(survivors);
                _population = newGeneration;
            }
        }

        #endregion

    }

}

