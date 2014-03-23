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

        #region File locations

        public const string GeneralLogFile = "log.txt";
        public const string EvolutionLogFile = "evolution.log.txt";

        #endregion

        #region Privat fields

        /// <summary>
        /// Logs general events
        /// </summary>
        private StreamWriter _generalLogWriter;

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
            if (File.Exists(GeneralLogFile))
            {
                File.Copy(GeneralLogFile, GeneralLogFile + ".bak", true);
            }
            if (File.Exists(EvolutionLogFile))
            {
                File.Copy(EvolutionLogFile, EvolutionLogFile + ".bak", true);
            }

           _generalLogWriter = new StreamWriter(GeneralLogFile) 
           { 
               AutoFlush = true,
               NewLine = "\r\n"
           };
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

        #region General logger

        /// <summary>
        /// Logs a message into the general log file
        /// </summary>
        public void LogInfo(string info)
        {
            Console.Out.WriteLine("INFO {0}", info);
            _generalLogWriter.WriteLine("{0} - {1,-10}: {2}", DateTime.Now, "INFO", info);
        }

        /// <summary>
        /// Logs a warning into the general log file
        /// </summary>
        public void LogWarning(string warning)
        {
            Console.Out.WriteLine("WARNING {0}", warning);
            _generalLogWriter.WriteLine("{0} - {1,-10}: {2}", DateTime.Now, "WARNING", warning);
        }

        /// <summary>
        /// Logs an error into the general log file
        /// </summary>
        public void LogError(string error)
        {
            Console.Out.WriteLine("ERROR {0}", error);
            _generalLogWriter.WriteLine("{0} - {1,-10}: {2}", DateTime.Now, "ERROR", error);
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
