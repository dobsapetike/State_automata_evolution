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

        private ExperimentProperties _properties;

        public ExperimentProperties Properties
        {
            get { return _properties; }
        }

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

            _properties = new ExperimentProperties();
            _properties.AddProperty("Number limit", 3);
            _properties.AddProperty("Zero probability", 0.75);
            _properties.AddProperty("Test cases", 750);
            _properties.AddProperty("Test string length", 10);
            SetCollections();
        }

        public void Reset()
        {
            SetCollections();
        }

        private void SetCollections()
        {
            var transl = new List<string>();
            var triggers = new List<TransitionTrigger>();
            for (var i = 0; i <= _properties["Number limit"]; ++i)
            {
                transl.Add(i + "");
                triggers.Add(new TransitionTrigger(i + ""));
            }
            TransitionTranslations = transl;
            TransitionEvents = triggers;
        }

        private string GenerateTestString()
        {
            var res = "";
            for (var i = 0; i < _properties["Test string length"]; ++i)
            {
                if (random.NextDouble() <= _properties["Zero probability"])
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
            SetCollections();
            _transducer = transducer;

            var score = 0d;
            var noTests = _properties["Test cases"];

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
            SetCollections();
            Console.WriteLine("******** No Zero Left Behind *********");
            Console.WriteLine(Description);
            Console.Write("Input: string containing characters of "
                + TransitionTranslations.Aggregate((x, y) => x + ", " + y));
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


