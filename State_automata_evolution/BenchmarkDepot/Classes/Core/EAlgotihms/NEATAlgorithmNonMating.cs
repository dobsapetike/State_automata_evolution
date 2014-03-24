namespace BenchmarkDepot.Classes.Core.EAlgotihms
{

    /// <summary>
    /// NEAT with ablated crossover mutation.
    /// </summary>
    public class NEATAlgorithmNonMating : NEATAlgorithm
    {

        public override string Name
        {
            get
            {
                return "Non-mating NEAT";
            }
        }

        public NEATAlgorithmNonMating() : base()
        {
            _generalParameters.CrossoverProbability = 0.0;
        }

    }

}
