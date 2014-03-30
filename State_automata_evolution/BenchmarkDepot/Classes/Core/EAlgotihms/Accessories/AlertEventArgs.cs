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

}
