using System;

namespace BenchmarkDepot.Classes.Core.EAlgotihms
{

    /// <summary>
    /// NEAT without speciation.
    /// The whole population is placed into one species, so the
    /// benefits of the speciation are no longer present. 
    /// </summary>
    public class NEATAlgorithmNonSpeciated : NEATAlgorithm
    {

        public override string Name
        {
            get
            {
                return "Non-Speciated NEAT";
            }
        }

        public NEATAlgorithmNonSpeciated() : base()
        {
            _neatParameters.MinCompatibilityThreshold = Int32.MaxValue;
        }

    }

}
