﻿using System;
using System.Linq;
using BenchmarkDepot.Classes.Core;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    public class BinaryTransducerExperiment : IExperiment
    {

        private Transducer _transducer;
        private Random random = new Random();

        private ExperimentProperties _properties;

        public ExperimentProperties Properties
        {
            get { return _properties; }
        }

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

            _properties = new ExperimentProperties();
            _properties.AddProperty("Test cases", 100);
            _properties.AddProperty("Test string length", 10);
        }

        public void Reset()
        {

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

            for (var i = 0; i < Properties["Test cases"]; ++i)
            {
                var testString = "";
                for (var j = 0; j < Properties["Test string length"]; ++j)
                    testString += random.Next(2) == 0 ? "0" : "1";
                score += TestCase(testString);
            }

            return score / Properties["Test cases"];            
        }

        public double RequiredFitness
        {
            get { return 1d; }
        }


        public void TestDrive(Transducer transducer)
        {
            Console.WriteLine("******** Binary transducer *********");
            Console.WriteLine(Description);
            Console.WriteLine("Input: string containing characters of '0' and '1'");
            Console.WriteLine(new String('*', 36) + '\n');
            for (;;)
            {
                Console.Write("> ");
                var input = Console.In.ReadLine();
                if (input.ToLower() == "exit") break;

                var modified = false;
                var inputMod = "";
                foreach (var c in input)
                {
                    if (TransitionTranslations.Contains(c+""))
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
