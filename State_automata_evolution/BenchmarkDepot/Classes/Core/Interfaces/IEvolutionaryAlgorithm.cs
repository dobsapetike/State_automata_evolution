using System.Collections.Generic;
using BenchmarkDepot.Classes.Core;

namespace BenchmarkDepot.Classes.Core.Interfaces
{

    public interface IEvolutionaryAlgorithm
    {
        string Name { get; }
        IExperiment Experiment { get; set; }
        Transducer Evolve();
    }

}
