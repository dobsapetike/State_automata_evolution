using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    public class TermostatExperiment : IExperiment
    {

        private const int minDegree = 0;
        private const int maxDegree = 100;
        private const int minIdealDegree = 50;
        private const int maxIdealDegree = 60;

        private Random _random = new Random();
        private Transducer _transducer;

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
    }

}

