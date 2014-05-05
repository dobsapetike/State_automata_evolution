using BenchmarkDepot.Classes.Core.EAlgotihms;
using BenchmarkDepot.Classes.Core.Experiments;
using BenchmarkDepot.Classes.Misc;
using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;
using System.Threading;
namespace BenchmarkDepot.Classes.Core
{

    public class QuickStartSetup
    {

        #region Private fields

        private int _runNumber = 10;
        private NEATAlgorithm _algorithm;
        private IExperiment _experiment;

        #endregion

        #region Constructor

        public QuickStartSetup(string filePath)
        {
            if (!File.Exists(filePath))
            {
                //Logger.CurrentLogger.LogError("Could not initialize quick start setup due to missing xml file!");
                throw new ArgumentException(filePath);
            }

            var xdoc = XDocument.Load(filePath);
            InitAlgorithm(xdoc.Root.Descendants("Algorithm").First());
            InitParams(xdoc.Root.Descendants("Parameters").First());
            InitExperiments(xdoc.Root.Descendants("ExperimentParameters").First());

            var runNumElem = xdoc.Root.Descendants("RunNumber").FirstOrDefault();
            if (runNumElem != null)
            {
                _runNumber = int.Parse(runNumElem.Value);
            }
        }

        #endregion

        #region Private methods

        private void InitAlgorithm(XElement element)
        {
            switch (int.Parse(element.Value))
            {
                case 1: 
                    _algorithm = new NEATAlgorithm(); 
                    break;
                case 2: 
                    _algorithm = new NEATAlgorithmInitialRandom(); 
                    break;
                case 3: 
                    _algorithm = new NEATAlgorithmCompleteStructure(); 
                    break;
                default:
                    throw new ArgumentException("Invalid algorithm number");
            }
        }

        private void InitParams(XElement element)
        {
            Action<XElement, object> SetParamValue = (elem, param) =>
            {
                foreach (var desc in elem.Descendants())
                {
                    // set the properties through reflection
                    var prop = param.GetType().GetProperty(desc.Name.LocalName);
                    var value = Convert.ChangeType(desc.Value, prop.PropertyType, CultureInfo.InvariantCulture);
                    prop.SetValue(param, value);
                }
            };

            SetParamValue(element.Descendants("GeneralParameters").First(), _algorithm.GeneralEAParameters);
            SetParamValue(element.Descendants("NEATParameters").First(), _algorithm.NEATParameters);
        }

        private void InitExperiments(XElement element)
        {
            var numDes = element.Descendants("ExperimentNumber").First();
            switch (int.Parse(numDes.Value))
            {
                case 1: _experiment = new BinaryTransducerExperiment(); break;
                case 2: _experiment = new NoZeroLeftBehindExperiment(); break;
                case 3: _experiment = new TermostatExperiment(); break;
                case 4: _experiment = new DressingRoomExperiment(); break;
                case 5: _experiment = new CatchMeIfYouCanExperiment(); break;
                default: throw new ArgumentException("Invalid experiment number");
            }
            numDes.Remove(); // so it won't be taken as property
            
            foreach (var desc in element.Descendants())
            {
                var name = desc.Name.LocalName.Replace("_", " ");
                _experiment.Properties[name] = Double.Parse(desc.Value);
            }
        }

        #endregion

        #region Public methods

        private bool _stillRunning = false;

        public void Start()
        {
            _algorithm.Experiment = _experiment;

            for (var i = 0; i < _runNumber; ++i)
            {
                while (_stillRunning) Thread.Sleep(10000);

                _stillRunning = true;
                _algorithm.Evolve(_ => { _stillRunning = false; });
            }
            while (_stillRunning) { } // don't exit until it's not done
        }

        #endregion

    }

}
