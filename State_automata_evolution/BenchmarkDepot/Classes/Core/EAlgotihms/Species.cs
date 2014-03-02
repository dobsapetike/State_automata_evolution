using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BenchmarkDepot.Classes.Core.EAlgotihms.Parameters;


namespace BenchmarkDepot.Classes.Core.EAlgotihms
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
        /// Parameters needed for calculating the compatibility distance
        /// </summary>
        private NEATParameters _neatParams;

        /// <summary>
        /// A transducer representing the whole species - used for measuring the compatibility
        /// distance
        /// </summary>
        private Transducer _representative;

        /// <summary>
        /// List of all transducers in the species
        /// </summary>
        private List<Transducer> _population;

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
            get { return _population.AsReadOnly(); }
        }

        #endregion

        #region Constructor

        public Species(int id, Transducer representative, NEATParameters parameters)
        {
            _id = id;
            _representative = representative;
            _neatParams = parameters;

            _population = new List<Transducer>();
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
            Func<Transducer, SortedDictionary<int, Tuple<HashSet<string>,HashSet<string>>>> GetInnovations = (T) =>
            {
                var result = new SortedDictionary<int, Tuple<HashSet<string>,HashSet<string>>>();
                foreach (var state in T.States)
                {
                    var trans = state.GetListOfTransitions();
                    foreach (var tran in trans)
                    {
                        var inNumb = tran.InnovationNumber;
                        if (!result.ContainsKey(inNumb))
                        {
                            result.Add(inNumb, new Tuple<HashSet<string>,HashSet<string>>
                                (new HashSet<string>(),new HashSet<string>()));
                        }
                        result[inNumb].Item1.Add(tran.TransitionEvent);
                        result[inNumb].Item2.Add(tran.ActionName);
                    }
                }
                return result;
            };

            var innovationsRep = GetInnovations(_representative);
            var innovationsNew = GetInnovations(t);

            if (innovationsRep.Count == 0 || innovationsNew.Count == 0) return false;

            int N = Math.Max(innovationsRep.Count, innovationsNew.Count);
            int maxInnovation = Math.Max(innovationsRep.Last().Key, innovationsNew.Last().Key);

            double weightDifference = 0.0;
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
                }

                if (isInRep)  excessCount++;
                else          disjointCount++;
            }

            var compDistance =
                (_neatParams.CoefExcessGeneFactor * excessCount) / N +
                (_neatParams.CoefDisjointGeneFactor * disjointCount) / N +
                _neatParams.CoefMatchingWeightDifferenceFactor * weightDifference;
            return compDistance <= _neatParams.CompatibilityThreshold;
        }

        /// <summary>
        /// Adds a new individual into the population if it's compatible
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
        /// Explicit fitness sharing - sets the adjusted fitness value for all the 
        /// individuals in the spiecies
        /// </summary>
        public void AdjustFitness()
        {
            var populationSize = _population.Count + 1;
            _representative.EvaluationInfo.AdjustedFitness = _representative.EvaluationInfo.Fitness / populationSize;
            for (int i = 0; i < _population.Count; ++i)
            {
                var fitness = _population[i].EvaluationInfo.Fitness;
                _population[i].EvaluationInfo.AdjustedFitness = fitness / populationSize;
            }
        }

        #endregion

    }

}
