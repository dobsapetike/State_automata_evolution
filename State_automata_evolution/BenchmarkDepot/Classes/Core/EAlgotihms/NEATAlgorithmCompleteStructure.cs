using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkDepot.Classes.Core.EAlgotihms
{

    /// <summary>
    /// TODO
    /// </summary>
    public class NEATAlgorithmCompleteStructure : NEATAlgorithm
    {

        public override string Name
        {
            get
            {
                return "Complete structure NEAT";
            }
        }

        protected override void InitializePopulation()
        {
            var rand = new Random();
            _stateIndex = _generalParameters.MaxIndividualSize;
            for (var i = 0; i < _generalParameters.InitialPopulationSize; ++i)
            {
                var t = new Transducer();

                var stateNo = _generalParameters.MaxIndividualSize;
                for (var j = 0; j < stateNo; ++j)
                {
                    t.AddState(new TransducerState(j));
                }
                // place a transition between every state
                for (var j = 0; j < stateNo; ++j)
                {
                    for (var k = 0; k < stateNo; ++k)
                    {
                        if (j == k) continue;

                        var trans = CreateRandomTransition(DetectInnovationNumberForConnection(j, k));
                        t.AddTransition(t.States[j], t.States[k], CreateRandomTrigger(), trans);
                    }
                }

                _population.Add(t);
            }
        }

    }

}
