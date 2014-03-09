using System.Collections.Generic;
using BenchmarkDepot.Classes.Core;

namespace BenchmarkDepot.Classes.Core.Interfaces
{

    /// <summary>
    /// General interface for the evolutionary algorithms
    /// </summary>
    public interface IEvolutionaryAlgorithm
    {
        /// <summary>
        /// Returns the name of the algorithm
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the reference to the current experiment
        /// </summary>
        IExperiment Experiment { get; set; }

        /// <summary>
        /// Starts the evolutionary cycle
        /// </summary>
        /// <returns>The resulting transducer</returns>
        Transducer Evolve();
    }

}
