using System;
using System.Linq;
using BenchmarkDepot.Classes.Core;
using BenchmarkDepot.Classes.Core.Interfaces;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core.Experiments
{
    public class Exp : IExperiment
    {
        //test
        private Random r;
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<TransitionTrigger> TransitionEvents { get; set; }
        public IEnumerable<Action> TransitionActions { get; set; }
        public IEnumerable<string> TransitionTranslations { get; set; }
        
        public double Run(Transducer transducer) 
        {
            transducer.Reset();
            
            const string s = "010111001011100";
            const string ss = "101000110100011";
            foreach (var c in s)
            {
                
                transducer.ShiftState(new TransitionTrigger(c+""));
            }

            var score = 0;
            for (var i = 0; i < ss.Length; ++i)
            {
                if (transducer.Translation.Length <= i) break;
                if (ss[i] == transducer.Translation[i]) ++score;
            }

            return (double)score / (double)ss.Length;
        }

        public Exp(string name, string description)
        {
            r = new Random();
            Name = name;
            Description = description;

            TransitionActions = new List<Action> { null };
            TransitionTranslations = new List<string> { "0", "1" };
            TransitionEvents = new List<TransitionTrigger> { new TransitionTrigger("0"), new TransitionTrigger("1") };
        }

        public double RequiredFitness
        {
            get { return 1.0; }
        }
    }
}
