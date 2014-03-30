using System;

namespace BenchmarkDepot.Classes.Core.EAlgotihms
{

    /// <summary>
    /// NEAT without the initial minimization.
    /// Each individual starts out with a random number of states
    /// and random number of transitions between them
    /// </summary>
    public class NEATAlgorithmInitialRandom : NEATAlgorithm
    {

        public override string Name
        {
            get
            {
                return "Initial random NEAT";
            }
        }

        public NEATAlgorithmInitialRandom() : base()
        {
            _generalParameters.StateDeletionMutationProbability = 0.6;
            _generalParameters.TransitionDeletionMutationProbability = 0.6;
        }

        protected override void InitializePopulation()
        {
            var rand = new Random();
            var maxStateCount = 0;

            for (int i = 0; i < _generalParameters.InitialPopulationSize; ++i)
            {
                var t = new Transducer();
                var stateNo = rand.Next(_generalParameters.MaxIndividualSize / 2,
                    _generalParameters.MaxIndividualSize);
                maxStateCount = Math.Max(maxStateCount, stateNo);

                var states = new TransducerState[stateNo];
                for (var j = 0; j < stateNo; ++j)
                {
                    states[j] = new TransducerState(j);
                }

                // now put there some transitions
                var transNo = rand.Next(stateNo, stateNo * 2);
                for (var j = 0; j < transNo; ++j)
                {
                    var first = rand.Next(stateNo);
                    var second = rand.Next(stateNo);
                    var attempt = 5;
                    while (attempt-- > 0)
                    {
                        if (first != second) break;
                        second = rand.Next(stateNo);
                    }

                    var trans = CreateRandomTransition(DetectInnovationNumberForConnection(first, second));
                    t.AddTransition(states[first], states[second], CreateRandomTrigger(), trans);
                }

                _population.Add(t);
            }

            _stateIndex = maxStateCount;
        }

    }

}
