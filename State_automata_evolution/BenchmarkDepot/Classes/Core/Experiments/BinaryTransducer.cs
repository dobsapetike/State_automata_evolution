using System;
using System.Linq;
using BenchmarkDepot.Classes.Core;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    public class BinaryTransducerExperiment : IExperiment
    {

        private Transducer _transducer;
        private Random random = new Random();

        public string Name 
        {
            get { return "Binary transducer"; }
        }

        public string Description 
        {
            get 
            { 
                return "Experiment for generating a simple transducer, which gets a " +
                    "string consisting of characters '0' and '1', and produces another " +
                    "string where zeros are replaced with ones and vice versa."; 
            } 
        }

        public IEnumerable<TransitionTrigger> TransitionEvents { get; private set; }
        public IEnumerable<Action> TransitionActions { get; private set; }
        public IEnumerable<string> TransitionTranslations { get; private set; }

        public BinaryTransducerExperiment()
        {
            TransitionActions = new List<Action> { null };
            TransitionTranslations = new List<string> { "0", "1" };
            TransitionEvents = new List<TransitionTrigger> { new TransitionTrigger("0"), new TransitionTrigger("1") };
        }

        private double TestCase(string input)
        {
            _transducer.Reset();

            foreach (var c in input)
            {
                _transducer.ShiftState(TransitionEvents.ElementAt(int.Parse(c.ToString())));
            }

            var score = 0;
            for (var i = 0; i < input.Length; ++i)
            {
                if (_transducer.Translation.Length <= i) break;
                if (input[i] != _transducer.Translation[i]) ++score;
            }

            return (double)score / (double)input.Length;
        }
        
        public double Run(Transducer transducer) 
        {
            _transducer = transducer;

            var score = 0d;

            var noTests = 5;
            for (var i = 0; i < noTests; ++i)
            {
                var testString = "";
                for (var j = 0; j < 10; ++j)
                    testString += random.Next(2) == 0 ? "0" : "1";
                score += TestCase(testString);
            }

            return score / (double)noTests;            
        }

        public double RequiredFitness
        {
            get { return 1d; }
        }
    }
}
