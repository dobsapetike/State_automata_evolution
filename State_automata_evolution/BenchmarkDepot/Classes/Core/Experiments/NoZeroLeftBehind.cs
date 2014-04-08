using System;
using System.Linq;
using BenchmarkDepot.Classes.Core;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    public class NoZeroLeftBehindExperiment : IExperiment
    {

        private Transducer _transducer;
        private Random random = new Random();
        private const double ZeroProbability = 0.75;

        public string Name
        {
            get { return "No Zero Left Behind"; }
        }

        public string Description
        {
            get
            {
                return "The goal is to find a transducer which translates every instance of " +
                    "the zero character to the last non-zero character.";
            }
        }

        public IEnumerable<TransitionTrigger> TransitionEvents { get; private set; }
        public IEnumerable<Action> TransitionActions { get; private set; }
        public IEnumerable<string> TransitionTranslations { get; private set; }

        public NoZeroLeftBehindExperiment()
        {
            TransitionActions = new List<Action> { null };
            TransitionTranslations = new List<string> { "0", "1", "2", "3" };
            TransitionEvents = new List<TransitionTrigger> 
            { 
                new TransitionTrigger("0"), 
                new TransitionTrigger("1"), 
                new TransitionTrigger("2"),
                new TransitionTrigger("3"),
            };
        }

        private string GenerateTestString()
        {
            var res = "";
            for (var i = 0; i < 10; ++i)
            {
                if (random.NextDouble() <= ZeroProbability)
                {
                    res += '0';
                }
                else
                {
                    res += random.Next(
                        1,TransitionTranslations.Count()) + "";
                }
            }
            return res;
        }

        private double TestCase(string input)
        {
            _transducer.Reset();

            foreach (var c in input)
            {
                _transducer.ShiftState(TransitionEvents.ElementAt(int.Parse(c.ToString())));
            }

            var score = 0;
            char lastControlChar = '0';
            for (var i = 0; i < input.Length; ++i)
            {
                if (_transducer.Translation.Length <= i) break;
                if (input[i] != '0') lastControlChar = input[i];
                if (_transducer.Translation[i] == lastControlChar) 
                    ++score;
                else 
                    break;
            }

            return (double)score / (double)input.Length;
        }

        public double Run(Transducer transducer)
        {
            _transducer = transducer;

            var score = 0d;
            var noTests = 500;

            for (var i = 0; i < noTests; ++i)
            {
                score += TestCase(GenerateTestString());
            }
            return score / (double)noTests;
        }

        public double RequiredFitness
        {
            get { return 1d; }
        }
    }
}

