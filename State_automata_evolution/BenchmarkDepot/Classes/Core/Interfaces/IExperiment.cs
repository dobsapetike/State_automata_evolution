using System;
using System.Collections.Generic;
using BenchmarkDepot.Classes.Core;

namespace BenchmarkDepot.Classes.Core.Interfaces
{

    /// <summary>
    /// Interface every experiment should implement
    /// </summary>
    public interface IExperiment
    {

        /// <summary>
        /// Returns the name of the experiment
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns a description of the experiment
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Returns the fitness limit a sufficiently evolved 
        /// automaton has to satisfy
        /// </summary>
        double RequiredFitness { get; }

        /// <summary>
        /// Enumerable of transition triggers the evolutionary algorithm
        /// can use during the evolution
        /// </summary>
        IEnumerable<TransitionTrigger> TransitionEvents { get; }

        /// <summary>
        /// Enumerable of transition transducers the evolutionary algorithm
        /// can use during the evolution
        /// </summary>
        IEnumerable<string> TransitionTranslations { get; }

        /// <summary>
        /// Enumerable of transtion actions the evolutionary algorithm 
        /// can use during the evolution
        /// </summary>
        IEnumerable<Action> TransitionActions { get; }

        /// <summary>
        /// Method for testing the quality of a transducer
        /// </summary>
        /// <returns>the acquired fitness score</returns>
        double Run(Transducer transducer);

    }

}
