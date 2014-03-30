using System;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;
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


        #endregion

        #region Constructor

        public EvolutionWindowViewModel(NEATAlgorithm algorithm)
        {
            Algorithm = algorithm;
            Algorithm.AlertEvent += OnAlert;
            IsEvolving = false;

            _alerts = new List<string>();
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
        }

        /// <summary>
        /// Called when user attempts to close the window
        /// </summary>
        public void OnViewClosing(object sender, CancelEventArgs e)
        {
            if (!IsEvolving) return;

            var res = MessageBox.Show("Evolution in progress. Would you like to abort it?", "Hey, I'm making science!",
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
        /// Called when the algorithm sends new aler.
        /// Saves the alert message for vizualization in view.
        /// </summary>
        private void OnAlert(object sender, AlertEventArgs args)
        {
            _alerts.Add(args.AlertMessage);
            RaisePropertyChanged(() => AlertList);
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
            Algorithm.Evolve(OnEvolutionComplete);
        }

        #endregion

        #region Paramter show command

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
                DataContext = new ParametersWindowViewModel(Algorithm.NEATParameters, Algorithm.GeneralEAParameters)
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

        #endregion

    }

}
