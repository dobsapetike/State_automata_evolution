using System;
using System.Linq;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    public class DressingRoomExperiment : IExperiment
    {

        private const int maxHeadCount = 3;
        private Random _random = new Random();

        public string Name
        {
            get { return "Dressing room"; }
        }

        public string Description
        {
            get { return "Description"; }
        }

        public double RequiredFitness
        {
            get { return 1.0; }
        }

        public IEnumerable<TransitionTrigger> TransitionEvents
        {
            get 
            {
                return new List<TransitionTrigger>
                {
                    new TransitionTrigger("Boy arrives"),
                    new TransitionTrigger("Girl arrives"),
                    new TransitionTrigger("Boy leaves"),
                    new TransitionTrigger("Girl leaves"),
                };
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

        public double Run(Transducer transducer)
        {
            transducer.Reset();

            const int testCase = 1000;
            int score = 0;
            int boyCount = 0, girlCount = 0;

            for (var i = 0; i < testCase; ++i)
            {
                var ok = true;
                var selected = _random.Next(4);
                var trig = TransitionEvents.ElementAt(selected);
                var suc = transducer.ShiftState(trig);

                switch(selected)
                {
                    case 0:
                        if (suc) ++boyCount;
                        if ((boyCount + girlCount > maxHeadCount) || (!suc && boyCount + girlCount + 1 <= maxHeadCount)) ok = false;
                        break;
                    case 1:
                        if (suc) ++girlCount;
                        if ((boyCount + girlCount > maxHeadCount) || (!suc && boyCount + girlCount + 1 <= maxHeadCount)) ok = false;
                        break;
                    case 2:
                        if (suc) --boyCount;
                        if ((boyCount + girlCount < 0) || (!suc && boyCount + girlCount - 1 >= 0)) ok = false;
                        break;
                    case 3:
                        if (suc) --girlCount;
                        if ((boyCount + girlCount < 0) || (!suc && boyCount + girlCount - 1 >= 0)) ok = false;
                        break;
                }

                if (ok)
                score++;
            }

            return (double)score / (double)testCase;
        }
    }

}
