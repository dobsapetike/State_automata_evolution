using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Misc = BenchmarkDepot.Classes.Misc;
using BenchmarkDepot.Classes.Core.EAlgotihms.Parameters;

namespace BenchmarkDepot.Classes.Core.EAlgotihms.Accessories
{

    /// <summary>
    /// Represents a species within a NEAT population
    /// </summary>
    public class Species
    {

        #region Private fields

        /// <summary>
        /// A unique id for every species
        /// </summary>
        private int _id;

        /// <summary>
        /// Parameters needed for calculating the compatibility distance and spawn count
        /// </summary>
        private NEATParameters _neatParams;
        private GeneralEAParameters _generalParam;

        /// <summary>
        /// A transducer representing the whole species - used for measuring the compatibility
        /// distance
        /// </summary>
        private Transducer _representative;

        /// <summary>
        /// List of all transducers in the species
        /// </summary>
        private Misc.SortedList<Transducer> _population;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the id of this species
        /// </summary>
        public int ID
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets a read only collection of all the individuals in this species
        /// </summary>
        public ReadOnlyCollection<Transducer> Population
        {
            get { return _population.ToList().AsReadOnly(); }
        }

        /// <summary>
        /// Number of generations no impovement occured in the species
        /// </summary>
        public int StagnatedGenerations
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets how many new transducers should be created for the new generation
        /// </summary>
        public int SpawnCount
        {
            get { return Convert.ToInt32(_population.Count * _neatParams.SurvivalRate); }
        }
        
        /// <summary>
        /// Property for setting a new representative
        /// </summary>
        public Transducer Representative
        {
            get
            {
                return _representative;
            }
            set 
            {
                if (_representative == value) return;
                _representative = value; 
            }
        }

        #endregion

        #region Constructor

        public Species(int id, Transducer representative, NEATParameters paramN, GeneralEAParameters paramG)
        {
            _id = id;
            _representative = representative;
            _neatParams = paramN;
            _generalParam = paramG;

            _population = new Misc.SortedList<Transducer> { _representative };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Detects if a given transducer is compatible with the species representative
        /// </summary>
        private bool IsCompatible(Transducer t)
        {
            
            // Returns a sorted collection of all innovations in a given transducer along with the 
            // transition actions and triggers
            Func<Transducer, SortedDictionary<int, Tuple<HashSet<TransitionTrigger>,HashSet<string>>>> GetInnovations = (T) =>
            {
                var result = new SortedDictionary<int, Tuple<HashSet<TransitionTrigger>,HashSet<string>>>();
                foreach (var state in T.States)
                {
                    var trans = state.GetListOfTransitions();
                    foreach (var tran in trans)
                    {
                        var inNumb = tran.InnovationNumber;
                        if (!result.ContainsKey(inNumb))
                        {
                            result.Add(inNumb, new Tuple<HashSet<TransitionTrigger>,HashSet<string>>
                                (new HashSet<TransitionTrigger>(),new HashSet<string>()));
                        }
                        result[inNumb].Item1.Add(tran.TransitionTrigger);
                        result[inNumb].Item2.Add(tran.ActionName);
                    }
                }
                return result;
            };

            if (!_neatParams.SpeciesAllowed) return true; // because this is the only species
                
            var innovationsRep = GetInnovations(_representative);
            var innovationsNew = GetInnovations(t);

            if (innovationsRep.Count == 0 || innovationsNew.Count == 0) 
                return false;

            int N = Math.Max(innovationsRep.Count, innovationsNew.Count);
            int maxInnovation = Math.Max(innovationsRep.Last().Key, innovationsNew.Last().Key);

            double weightDifference = 0d;
            int excessCount = 0, disjointCount = 0;
            for (int i = 0; i < maxInnovation; ++i)
            {
                var isInRep = innovationsRep.ContainsKey(i);
                var isInNew = innovationsNew.ContainsKey(i);

                if (!(isInRep || isInNew)) continue;
                if (isInRep && isInNew)
                {
                    if (innovationsRep[i].Item1.Intersect(innovationsNew[i].Item1).Count() > 0)
                    {
                        weightDifference += _neatParams.MatchingWeightDifferenceValue / 2;
                    }
                    if (innovationsRep[i].Item2.Intersect(innovationsNew[i].Item2).Count() > 0)
                    {
                        weightDifference += _neatParams.MatchingWeightDifferenceValue / 2;
                    }
                    continue;
                }

                if (isInRep)  excessCount++;
                else          disjointCount++;
            }

            var compDistance =
                (_neatParams.CoefExcessGeneFactor * excessCount) / N +
                (_neatParams.CoefDisjointGeneFactor * disjointCount) / N +
                _neatParams.CoefMatchingWeightDifferenceFactor * weightDifference;
            compDistance = compDistance / (double) N;
            return compDistance <= _neatParams.CompatibilityThreshold;
        }

        /// <summary>
        /// Adds a new individual into the population if it's compatible with the representative
        /// </summary>
        /// <returns>whether the insertion was successful</returns>
        public bool InsertNew(Transducer t)
        {
            var compatible = IsCompatible(t);
            if (compatible)
            {
                _population.Add(t);
            }
            return compatible;
        }

        /// <summary>
        /// Called at the start of every generation.
        /// Increases the age of every transducer
        /// </summary>
        public void GenerationStart()
        {
            foreach (var p in _population)
            {
                p.EvaluationInfo.HappyBirthday();
            }
        }

        /// <summary>
        /// Checks whether the spieces has enough members - if not, mark this generation as stagnating.
        /// Should be called only once per generation, at the end.
        /// </summary>
        public void CheckStagnation()
        {
            if (_population.Count <= 1) StagnatedGenerations = _neatParams.AllowedSpeciesStagnatedGenerationCount + 1;
            if (_population.Count < Convert.ToInt32(_neatParams.CriticalSpecieCount * 0.2))
            {
                ++StagnatedGenerations;
            }
        }

        /// <summary>
        /// Sets the fittest member as the representative
        /// </summary>
        public void SelectNewRepresentative()
        {
            var best = _population.Last();
            if (best != _representative)
            {
                _representative = best;
            }
        }

        /// <summary>
        /// Method for clearing the population at the end of every cycle
        /// The representative stays in the species
        /// </summary>
        public void Clear()
        {
            _population.Clear();
            _population.Add(_representative);
        }

        /// <summary>
        /// Returns a random, but sufficiently fit candidate for 
        /// mutation/crossover
        /// </summary>
        public Transducer GetCandidateForMultiplication()
        {
            if (_population.Count == 1) return _representative;

            var r = new Random();
            int amount = Convert.ToInt32(_population.Count * _neatParams.SurvivalRate);
            // the best individuals are at the end of the collection
            int selected = r.Next(_population.Count - amount, _population.Count);

            return _population[selected];
        }

        /// <summary>
        /// Explicit fitness sharing - sets the adjusted fitness value for all the 
        /// individuals in the species
        /// </summary>
        public void AdjustFitness()
        {
            var populationSize = _population.Count;
            foreach (var individ in _population)
            {
                var fitness = individ.EvaluationInfo.Fitness;
                individ.EvaluationInfo.AdjustedFitness = fitness / (double)populationSize;
            }
        }

        #endregion

    }

}
