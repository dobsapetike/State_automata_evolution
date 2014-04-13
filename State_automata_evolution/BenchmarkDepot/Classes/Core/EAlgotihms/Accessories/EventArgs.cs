namespace BenchmarkDepot.Classes.Core.EAlgotihms.Accessories
{
    /// <summary>
    /// Describes arguments an alert event sends
    /// </summary>
    public class AlertEventArgs
    {
        public string AlertMessage { get; private set; }
        public AlertEventArgs(string alert)
        {
            AlertMessage = alert;
        }
    }

    /// <summary>
    /// Arguments the 'GenerationEnd' event sends - which are important
    /// stats describing the current evolution
    /// </summary>
    public class GenerationEndArgs
    {
        public int Generation { get; set; }
        public double BestFitness { get; set; }
        public long EvaliationCount { get; set; }
    }

}
