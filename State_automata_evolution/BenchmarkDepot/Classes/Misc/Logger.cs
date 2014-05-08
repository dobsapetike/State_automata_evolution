using System;
using System.IO;
using BenchmarkDepot.Classes.Core;

namespace BenchmarkDepot.Classes.Misc
{

    /// <summary>
    /// Singleton class for logging events during runtime - general events as well as evolution info
    /// </summary>
    public class Logger
    {

        #region File location

        public const string GraphDataOutputPath = "output/";
        public const string EvolutionLogFile = "evolution.log.txt";

        #endregion

        #region Privat fields

        /// <summary>
        /// Logs data for graph creation
        /// </summary>
        private StreamWriter _graphDataWriter;

        /// <summary>
        /// Logs evolution data
        /// </summary>
        private StreamWriter _evolutionLogWriter;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor initializes the streamwriters and creates backups 
        /// of existing log files
        /// </summary>
        private Logger()
        {
            if (File.Exists(EvolutionLogFile))
            {
                File.Copy(EvolutionLogFile, EvolutionLogFile + ".bak", true);
            }
            if (!Directory.Exists(GraphDataOutputPath)) Directory.CreateDirectory(GraphDataOutputPath);

           _evolutionLogWriter = new StreamWriter(EvolutionLogFile) 
           { 
               AutoFlush = true,
               NewLine = "\r\n"
           };
        }

        #endregion

        #region Instance

        private static Logger _logger;

        /// <summary>
        /// Gets the static singleton instance of the logger
        /// </summary>
        public static Logger CurrentLogger
        {
            get { return _logger ?? (_logger = new Logger()); }
        }

        #endregion

        #region Graphics data logger

        /// <summary>
        /// Counter for the output file name
        /// </summary>
        private int _graphFileCounter = 0;

        /// <summary>
        /// Called at the start of a new generation, created new file output
        /// </summary>
        private void SwitchGraphFile()
        {
            _graphDataWriter = new StreamWriter(GraphDataOutputPath + ++_graphFileCounter + ".txt")
            {
                AutoFlush = true,
                NewLine = "\r\n"
            };
        }

        /// <summary>
        /// Logs the graph data
        /// </summary>
        public void LogGraphData(string data)
        {
            _graphDataWriter.WriteLine(data);
        }

        #endregion

        #region Evolution logger

        /// <summary>
        /// Logs the start of a new evolution
        /// </summary>
        /// <param name="algorithmTitle">name of the evolutionary algorithm</param>
        /// <param name="experimentTitle">name of the experiment</param>
        public void LogEvolutionStart(string algorithmTitle, string experimentTitle)
        {
            SwitchGraphFile();
            Console.Out.WriteLine("Evolution start ({0}, {1})", algorithmTitle, experimentTitle);
            _evolutionLogWriter.WriteLine(new String('*', 80));
            _evolutionLogWriter.WriteLine("*" + new String(' ', 78) + "*");
            _evolutionLogWriter.WriteLine("* {0,-36} // {1,36} *", algorithmTitle, experimentTitle);
            _evolutionLogWriter.WriteLine("*" + new String(' ', 78) + "*");
            _evolutionLogWriter.WriteLine(new String('*', 80) + "\r\n");
        }

        /// <summary>
        /// Legs the end of the generation with a short summary
        /// </summary>
        /// <param name="generation">generations passed</param>
        /// <param name="result">string representation of the resulting transducer</param>
        /// <param name="found">whether the resulting transducer is a good enough solution </param>
        public void LogEvolutionEnd(int generation, string result, bool found)
        {
            Console.Out.WriteLine("Evolution end\nResult:\n{0}", result);
            _evolutionLogWriter.WriteLine("\r\nProcess over!");
            _evolutionLogWriter.WriteLine("{0,-25}:{1}", "Generation", generation);
            _evolutionLogWriter.WriteLine("{0,-25}:{1}", "Sufficient solution", found);
            _evolutionLogWriter.WriteLine("Result:\r\n{0}\r\n\r\n", result.Replace("\n","\r\n"));

            _graphDataWriter.Close();
            _graphDataWriter.Dispose();
        }

        /// <summary>
        /// Logs evolution data
        /// </summary>
        /// <param name="key">data name</param>
        /// <param name="value">data value</param>
        public void LogStat(string key, double value, string prefix = "")
        {
            Console.Out.WriteLine(prefix + "{0,-25}:{1}", key, value);
            _evolutionLogWriter.WriteLine(prefix + "{0,-25}:{1}", key, value);
        }

        #endregion

    }

}
