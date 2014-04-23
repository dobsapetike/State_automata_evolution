using System;
using System.Linq;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    public class DressingRoomExperiment : IExperiment
    {

        private int _maxHeadCount = 2, _testCases = 1000;
        private Random _random = new Random();

        private ExperimentProperties _properties;

        public ExperimentProperties Properties
        {
            get { return _properties; }
        }

        public string Name
        {
            get { return "Dressing room"; }
        }

        public string Description
        {
            get { return "Automat simulates a dressing room - when a new costumer arrives - tells "
                + "him whether there is free space or not."; }
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
                    new TransitionTrigger("boy arrives"),
                    new TransitionTrigger("girl arrives"),
                    new TransitionTrigger("boy leaves"),
                    new TransitionTrigger("girl leaves"),
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

        public void Reset()
        {
            _maxHeadCount = (int)_properties["Head count"];
            _testCases = (int)_properties["Test cases"];
        }

        public DressingRoomExperiment()
        {
            _properties = new ExperimentProperties();
            _properties.AddProperty("Test cases", 1000d);
            _properties.AddProperty("Head count", 2d);
        }

        public double Run(Transducer transducer)
        {
            transducer.Reset();

            int score = 0;
            int boyCount = 0, girlCount = 0;

            for (var i = 0; i < _testCases; ++i)
            {
                var ok = true;
                var selected = _random.Next(4);
                var trig = TransitionEvents.ElementAt(selected);
                var suc = transducer.ShiftState(trig);

                switch(selected)
                {
                    case 0:
                        if (suc) ++boyCount;
                        if ((boyCount + girlCount > _maxHeadCount) || (!suc && boyCount + girlCount + 1 <= _maxHeadCount)) ok = false;
                        break;
                    case 1:
                        if (suc) ++girlCount;
                        if ((boyCount + girlCount > _maxHeadCount) || (!suc && boyCount + girlCount + 1 <= _maxHeadCount)) ok = false;
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

                if (ok) score++;
            }

            return (double)score / (double)_testCases;
        }

        public void TestDrive(Transducer transducer)
        {
            Console.WriteLine("******** Dressing room *********");
            Console.WriteLine(Description);
            Console.WriteLine("Input: string containing two words with space separator: \"a b\"" +
                "\n\ta == girl v boy "
                +"\n\tb == arrives v leaves");
            Console.WriteLine(new String('*', 36) + '\n');
            HashSet<string> firstS = new HashSet<string> { "girl", "boy" },
                secondS = new HashSet<string> { "arrives", "leaves" };

            transducer.Reset();
            for (;;)
            {
                Console.Write("> ");
                var input = Console.In.ReadLine();
                if (input.ToLower() == "exit") break;

                var s = input.Split();
                if (s.Length != 2 || !firstS.Contains(s[0]) || !secondS.Contains(s[1]))
                {
                    Console.WriteLine("Error, wrong input!");
                    continue;
                }

                var shift = transducer.ShiftState(new TransitionTrigger(s[0] + " " + s[1]));
                Console.Out.WriteLine("< " + (shift ? "OK" : "Not OK"));
            }
        }

    }

}
