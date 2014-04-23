using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    public class TermostatExperiment : IExperiment
    {

        private Random _random = new Random();
        private Transducer _transducer;

        private int minDegree, maxDegree, minIdealDegree, maxIdealDegree;

        private ExperimentProperties _properties;

        public ExperimentProperties Properties
        {
            get { return _properties; }
        }

        public string Name
        {
            get { return "Termostat"; }
        }

        public string Description
        {
            get { return "Experiment for demonstrating conditional transitions, ie. transitions, that "+
                "are activated when their event is raised and at the same time a corresponding condition is met. In this case " +
                "the transducer is trying to keep the temperature between 50 and 60 degrees."; }
        }

        public double RequiredFitness
        {
            get { return 1d; }
        }

        public void Reset()
        {
            minDegree = (int)_properties["Min degree"];
            maxDegree = (int)_properties["Max degree"];
            minIdealDegree = (int)_properties["Min ideal degree"];
            maxIdealDegree = (int)_properties["Max ideal degree"];
        }

        public IEnumerable<TransitionTrigger> TransitionEvents
        {
            get
            {
                return new List<TransitionTrigger> { new TransitionTrigger("SetDegree", true) 
                    { MinParameterValue = minDegree, MaxParameterValue = maxDegree }};
            }
        }

        public IEnumerable<string> TransitionTranslations
        {
            get { return new List<string> { "X" }; }
        }

        public IEnumerable<Action> TransitionActions
        {
            get { return new List<Action> { null }; }
        }

        public TermostatExperiment()
        {
            _properties = new ExperimentProperties();
            _properties.AddProperty("Min degree", 0);
            _properties.AddProperty("Max degree", 100);
            _properties.AddProperty("Min ideal degree", 50);
            _properties.AddProperty("Max ideal degree", 60);
        }

        private double TestCase(int testVal = -1)
        {
            var trLength = _transducer.Translation.Length;
            int testDegree = testVal;
            if (testVal == -1)
            {
                if (_random.Next(2) == 0)
                    testDegree = _random.Next(minDegree, maxDegree+1);
                else
                    testDegree = _random.Next(minIdealDegree, maxIdealDegree+1);
            }
            var ok = testDegree >= minIdealDegree && testDegree <= maxIdealDegree;

            _transducer.ShiftState(TransitionEvents.First(), testDegree);
            var curLength = _transducer.Translation.Length;

            return ok ? trLength == curLength ? 0d : 1d
                      : trLength == curLength ? 1d : 0d;
        }

        public double Run(Transducer transducer)
        {
            transducer.Reset();
            _transducer = transducer;

            var score = 0d;
            var testCases = 300;
            for (var i = 0; i < testCases; ++i)
            {
                score += TestCase();
            }
            score += TestCase(50);
            score += TestCase(56);
            score += TestCase(59);
            score += TestCase(61);
            score += TestCase(49);

            return (double)score / (double)(testCases+5);
        }


        public void TestDrive(Transducer transducer)
        {
            Console.WriteLine("******** Termostat *********");
            Console.WriteLine(Description);
            Console.WriteLine("Input: degree as an integer");
            Console.WriteLine(new String('*', 36) + '\n');

            transducer.Reset();
            for (;;)
            {
                Console.Write("> ");
                var input = Console.In.ReadLine();
                if (input.ToLower() == "exit") break;

                double degree;
                var res = Double.TryParse(input, out degree);
                if (!res)
                {
                    Console.WriteLine("That's not it, I need a number!");
                    continue;
                }

                var shift = transducer.ShiftState(TransitionEvents.First(), degree);
                Console.Out.WriteLine("< " + (shift ? "OK" : "Not OK"));
            }
        }

    }

}

