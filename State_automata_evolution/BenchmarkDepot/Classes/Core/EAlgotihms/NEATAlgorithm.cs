using System;
using System.Collections.Generic;
using System.Linq;
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

        protected NEATParameters _neatParameters = new NEATParameters();
        protected GeneralEAParameters _generalParameters = new GeneralEAParameters();

        #endregion

        /// <summary>
        /// List of all innovations that occurred during the current generation
        /// </summary>
        protected List<Innovation> _innovations = new List<Innovation>();

        /// <summary>
        /// List of all species in the population
        /// </summary>
        protected List<Species> _population = new List<Species>();

        private Random random = new Random();

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
        /// Inserts a new transducer into a fitting species
        /// If no suitable specie exists a new is created   
        /// </summary>
        protected void InsertNewTransducer(Transducer t)
        {
            foreach (var species in _population)
            {
                if (species.InsertNew(t))
                {
                    return;
                }
            }
            var newSpecies = new Species(_speciesIndex++, t, _neatParameters);
            _population.Add(newSpecies);
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
                t.AddState(new TransducerState(1));
                t.AddState(new TransducerState(2));
                InsertNewTransducer(t);
            }
            _stateIndex = 2;
        }

        /// <summary>
        /// Performs a mutation on a transducer
        /// </summary>
        /// <returns>the mutated transducer</returns>
        protected virtual Transducer MutateTransducer(Transducer transducer)                  
        {
            // Creates a random transition with given innovation number
            Func<int, TransducerTransition> CreateRandomTransition = (innovation) =>
            {
                return new TransducerTransition(
                    Experiment.TransitionActions.ElementAt(random.Next(Experiment.TransitionActions.Count())),
                    Experiment.TransitionTranslations.ElementAt(random.Next(Experiment.TransitionTranslations.Count())),
                    innovation
                );
            };

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

            AddConnection(mutated, notConnected);
            AddNode(mutated, connected);

            return mutated;
        }

        /// <summary>
        /// Performs a crossover of two transducers
        /// </summary>
        /// <param name="a">first parent</param>
        /// <param name="b">second parent</param>
        /// <returns>the resulting offspring</returns>
        protected virtual Transducer CrossTransducers(Transducer a, Transducer b)        
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
            
            SaveTransitions(a, transitions);
            SaveTransitions(b, transitions);

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

            for (;;)
            {
                if (++_generation == _generalParameters.GenerationThreshold)
                {
                    // finalizations
                    return null;
                }
                // perform evaluation, selection, crossover, mutation
                // replace population
                return null;
            }
        }

        #endregion

    }

}

