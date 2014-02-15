using BenchmarkDepot.Classes.Core;
using BenchmarkDepot.Classes.Core.Interfaces;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core.Experiments
{
    public class Exp : IExperiment
    {
        //test
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> TransitionEvents { get; set; }
        public int Run(Transducer transducer) { return 0; }
        public Exp(string name, string description)
        {
            Name = name;
            Description = description;
        }


        public IEnumerable<System.Action> TransitionActions
        {
            get;
            set; 
        }


        public IEnumerable<string> TransitionTranslations
        {
            get;
            set;
        }
    }
}
