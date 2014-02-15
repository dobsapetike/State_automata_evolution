using System.Collections.Generic;
using BenchmarkDepot.Classes.Core.Interfaces;
using BenchmarkDepot.Classes.Core;

namespace BenchmarkDepot.Classes.Core.EAlgotihms
{
    //dummy class - will be deleted
    public class Evol : IEvolutionaryAlgorithm
    {
        public string Name { get; set; }
        public Transducer Evolve()
        {
            return null;
        }
        public Evol(string name) { Name = name; }
        public override string ToString()
        {
            return Name;
        }


        public IExperiment Experiment
        {
            get;
            set;
        }
    }
}
