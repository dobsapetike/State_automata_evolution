using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BenchmarkDepot.Classes.Core.Experiments;
using BenchmarkDepot.Classes.Core.EAlgotihms;
using BenchmarkDepot.Classes.Core.EAlgotihms.Accessories;
using BenchmarkDepot.Classes.Misc;

namespace BenchmarkDepot.Classes.GUI.ViewModel
{

    public class EvolutionWindowViewModel : ObservableObject
    {

        #region Properties

        /// <summary>
        /// Gets the selected evolutionary algorithm
        /// </summary>
        public NEATAlgorithm Algorithm { get; private set; }

        /// <summary>
        /// Gets the selected experiment
        /// </summary>
        public IExperiment Experiment 
        {
            get { return Algorithm.Experiment; }
        }

        private bool _isEvolving;

        /// <summary>
        /// Gets whether evolution is in progress
        /// </summary>
        public bool IsEvolving
        {
            get { return _isEvolving; }
            private set
            {
                _isEvolving = value;
                RaisePropertyChanged(() => IsEvolving);
            }
        }

        private List<string> _alerts;

        /// <summary>
        /// The list of alert messages represented as a string
        /// </summary>
        public string AlertList
        {
            get
            {
                if (!_alerts.Any()) return "Alerts list!";
                return _alerts.Aggregate((current, x) => current + "\r\n" + x);
            }
        }

        #region Graph data

        /// <summary>
        /// Collection of key-best fitness value pairs for visualisation
        /// </summary>
        public ObservableCollection<KeyValuePair<long, double>> GraphData { get; private set; }

        /// <summary>
        /// Collection of generation-best fitness pairs
        /// </summary>
        private ObservableCollection<KeyValuePair<long, double>> _graphGeneration 
            = new ObservableCollection<KeyValuePair<long, double>>();

        private bool _graphGenerationShow;

        public bool IsGrapGenerationShow
        {
            get { return _graphGenerationShow; }
            set
            {
                _graphGenerationShow = value;
                if (_graphGenerationShow)
                {
                    GraphData = _graphGeneration;
                    RaisePropertyChanged(() => GraphData);
                }
                RaisePropertyChanged(() => IsGrapGenerationShow);
            }
        }

        /// <summary>
        /// Collection of evaluation count-best fitness paris
        /// </summary>
        private ObservableCollection<KeyValuePair<long, double>> _graphEvaluation 
            = new ObservableCollection<KeyValuePair<long,double>>();

        private bool _graphEvaluationShow;

        public bool IsGrapEvaluationShow
        {
            get { return _graphEvaluationShow; }
            set
            {
                _graphEvaluationShow = value;
                if (_graphEvaluationShow)
                {
                    GraphData = _graphEvaluation;
                    RaisePropertyChanged(() => GraphData);
                }
                RaisePropertyChanged(() => IsGrapEvaluationShow);
            }
        }

        #endregion

        #endregion

        #region Constructor

        public EvolutionWindowViewModel(NEATAlgorithm algorithm)
        {
            Algorithm = algorithm;
            Algorithm.AlertEvent += OnAlert;
            Algorithm.GenerationEndEvent += OnGenerationEnd;
            Algorithm.Reset();
            IsEvolving = false;

            _alerts = new List<string>();
            GraphData = new ObservableCollection<KeyValuePair<long, double>>();
            IsGrapGenerationShow = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when evolution is complete
        /// </summary>
        /// <param name="asyncResult">always null, needed for compatibility reasons</param>
        private void OnEvolutionComplete(IAsyncResult asyncResult)
        {
            IsEvolving = false;
            App.Current.Dispatcher.Invoke(() => 
                _testDriveCommand.RaiseCanExecuteChanged());
        }

        /// <summary>
        /// Called when user attempts to close the window
        /// </summary>
        public void OnViewClosing(object sender, CancelEventArgs e)
        {
            if (!IsEvolving) return;

            var res = MessageBox.Show("Evolution in progress. Would you like to abort it?", "Hey, I'm making science here!",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (res == MessageBoxResult.Yes)
            {
                AbortEvolutionCommand.Execute(null);
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Called when the algorithm sends new alert.
        /// Saves the alert message for vizualization in view.
        /// </summary>
        private void OnAlert(object sender, AlertEventArgs args)
        {
            _alerts.Add(args.AlertMessage);
            RaisePropertyChanged(() => AlertList);
        }

        /// <summary>
        /// Called at the end of each generation
        /// </summary>
        private void OnGenerationEnd(object sender, GenerationEndArgs args)
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                var g = args.Generation;
                var f = args.BestFitness;
                var e = args.EvaliationCount;
                _graphGeneration.Add(new KeyValuePair<long, double>(g, f));
                _graphEvaluation.Add(new KeyValuePair<long, double>(e, f));
            }));
        }

        #endregion

        #region Commands

        #region Evolution command

        private  DelegateCommand _evolveCommand;

        /// <summary>
        /// Starts the evolution
        /// </summary>
        public ICommand EvolutionCommand
        {
            get
            {
                return _evolveCommand ?? (_evolveCommand =
                    new DelegateCommand(OnEvolveCommand, _ => true));
            }
        }

        private void OnEvolveCommand(object value)
        {
            IsEvolving = true;
            _graphEvaluation.Clear();
            _graphGeneration.Clear();
            Experiment.Reset();
            Algorithm.Evolve(OnEvolutionComplete);
        }

        #endregion

        #region Parameter show command

        private DelegateCommand _paramWinShowCommand;

        /// <summary>
        /// Shows the window with the parameter settings
        /// </summary>
        public ICommand ShowParamsCommand
        {
            get
            {
                return _paramWinShowCommand ?? (_paramWinShowCommand =
                    new DelegateCommand(OnParamWinShowCommand, _ => true));
            }
        }

        private void OnParamWinShowCommand(object value)
        {
            var paramWin = new ParametersWindow
            {
                DataContext = new ParametersWindowViewModel(Algorithm)
            };
            paramWin.ShowDialog();
        }

        #endregion

        #region Abort command

        private DelegateCommand _abortEvolutionCommand;

        /// <summary>
        /// Stops the evolution
        /// </summary>
        public ICommand AbortEvolutionCommand
        {
            get
            {
                return _abortEvolutionCommand ?? (_abortEvolutionCommand =
                    new DelegateCommand(OnAbortCommand, _ => true));
            }
        }

        private void OnAbortCommand(object value)
        {
            if (!IsEvolving) return;

            _alerts.Add("Evolution abortion initialized! Hang tight!");
            RaisePropertyChanged(() => AlertList);

            Algorithm.RequestStopEvolution();
        }

        #endregion

        #region Test drive command

        private DelegateCommand _testDriveCommand;

        /// <summary>
        /// Performs the user controlled test of the transducer
        /// </summary>
        public ICommand TestDriveCommand
        {
            get
            {
                return _testDriveCommand ?? (_testDriveCommand =
                    new DelegateCommand(OnTestDriveCommand, CanTestDrive));
            }
        }

        private bool CanTestDrive(object value)
        {
            return Algorithm.EvolutionResult != null;
        }

        private void OnTestDriveCommand(object value)
        {
            Console.Clear();
            ConsoleManager.ShowConsole();
            Algorithm.Experiment.TestDrive(Algorithm.EvolutionResult);
            ConsoleManager.HideConsole();
        }

        #endregion

        #region Save image command

        private DelegateCommand _saveImageCommand;

        /// <summary>
        /// Saves chart pic as png
        /// </summary>
        public ICommand SaveImageCommand
        {
            get
            {
                return _saveImageCommand ?? (_saveImageCommand =
                    new DelegateCommand(OnSaveImageCommand, _ => true));
            }
        }

        private void OnSaveImageCommand(object value)
        {
            var visual = value as FrameworkElement;
            if (visual == null) return;

            int pngWidth = (int)visual.ActualWidth;
            int pngHeight = (int)visual.ActualHeight;
            var bitmap = new RenderTargetBitmap(
                pngWidth, pngHeight,
                96, 96,
                PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(frame);

            var sfd = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = "EvolutionChart",
                    DefaultExt = ".png",
                };
            if (sfd.ShowDialog() == null) return;
            using (var stream = File.Create(sfd.FileName))
            {
                encoder.Save(stream);
            }
        }

        #endregion

        #endregion

    }

}
