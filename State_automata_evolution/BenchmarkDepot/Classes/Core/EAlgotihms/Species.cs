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
            
            // Returns a sorted collection of all innovations in a given transducer
            Func<Transducer, SortedSet<int>> GetInnovations = (T) =>
            {
                var result = new SortedSet<int>();
                foreach (var state in T.States)
                {
                    var trans = state.GetListOfTransitions();
                    foreach (var tran in trans)
                    {
                        result.Add(tran.InnovationNumber);
                    }
                }
                return result;
            };

            var innovationsRep = GetInnovations(_representative);
            var innovationsNew = GetInnovations(t);

            if (innovationsRep.Count == 0 || innovationsNew.Count == 0) return false;

            int N = Math.Max(innovationsRep.Count, innovationsNew.Count);
            int maxInnovation = Math.Max(innovationsRep.Last(), innovationsNew.Last());

            int excessCount = 0, disjointCount = 0;
            for (int i = 0; i < maxInnovation; ++i)
            {
                var isInRep = innovationsRep.Contains(i);
                var isInNew = innovationsNew.Contains(i);

                if (!(isInRep || isInNew)) continue;
                if (isInRep && isInNew)
                {
                    // TODO weight difference - but we don't have weights!
                }

                if (isInRep)  excessCount++;
                else          disjointCount++;
            }

            var compDistance =
                (_neatParams.CoefExcessGeneFactor * excessCount) / N +
                (_neatParams.CoefDisjointGeneFactor * disjointCount) / N +
                0;  //_neatParams.CoefMatchingWeightDifferenceFactor * weightDifference
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

        #endregion

    }

}
