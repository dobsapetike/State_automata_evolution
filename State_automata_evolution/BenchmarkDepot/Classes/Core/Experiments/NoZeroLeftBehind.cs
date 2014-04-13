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
            var noTests = 350;

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

        public void TestDrive(Transducer transducer)
        {
            Console.WriteLine("******** No Zero Left Behind *********");
            Console.WriteLine(Description);
            Console.Write("Input: string containing characters of ");
            foreach (var c in TransitionTranslations)
            {
                Console.Write(c + " ");
            }
            Console.WriteLine('\n' + new String('*', 40) + '\n');
            for (; ; )
            {
                Console.Write("> ");
                var input = Console.In.ReadLine();
                if (input.ToLower() == "exit") break;

                var modified = false;
                var inputMod = "";
                foreach (var c in input)
                {
                    if (TransitionTranslations.Contains(c + ""))
                    {
                        inputMod += c;
                        continue;
                    }
                    modified = true;
                }
                if (modified)
                {
                    Console.WriteLine("Sorry, invalid input, I had to change it!");
                    Console.WriteLine("> " + inputMod);
                }

                transducer.Reset();
                foreach (var c in inputMod)
                {
                    transducer.ShiftState(TransitionEvents.ElementAt(int.Parse(c.ToString())));
                }
                Console.Out.WriteLine("< " + transducer.Translation);
            }
        }

    }
}

