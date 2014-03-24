using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using BenchmarkDepot.Classes.Core;
using BenchmarkDepot.Classes.Core.Interfaces;
using BenchmarkDepot.Classes.Extensions;
using BenchmarkDepot.Classes.Core.EAlgotihms;
using BenchmarkDepot.Classes.Core.Experiments;

namespace BenchmarkDepot.Classes.ViewModel
{

    public class BenchmarkDepotViewModel : BaseViewModel
    {

        #region Private fields

        private short _currentPosition = 0;

        #endregion

        #region Properties

        #region Algorithms

        public ObservableCollection<IEvolutionaryAlgorithm> Algorithms { get; private set; }

        private IEvolutionaryAlgorithm _currentAlgorithm;

        public IEvolutionaryAlgorithm CurrentAlgorithm
        {
            get { return _currentAlgorithm; }
            set
            {
                if (_currentAlgorithm == value) return;
                _currentAlgorithm = value;
                RaisePropertyChanged(() => CurrentAlgorithm);
            }
        }

        #endregion

        #region Experiments

        public ObservableCollection<IExperiment> Experiments { get; private set; }

        private IExperiment _currentExperiment;

        public IExperiment CurrentExperiment
        {
            get { return _currentExperiment; }
            set
            {
                if (_currentExperiment == value) return;
                _currentExperiment = value;
                RaisePropertyChanged(() => CurrentExperiment);
            }
        }

        #endregion

        #endregion

        #region Constructor

        public BenchmarkDepotViewModel()
        {
            Algorithms = new ObservableCollection<IEvolutionaryAlgorithm>
                {
                    new NEATAlgorithm(),
                    new NEATAlgorithmInitialRandom(),
                    new NEATAlgorithmNonMating(),
                    new NEATAlgorithmNonSpeciated(),
                };
            CurrentAlgorithm = Algorithms[0];

            Experiments = new ObservableCollection<IExperiment>
            {
                new BinaryTransducerExperiment(),
            };
            CurrentExperiment = Experiments[0];
        }

        #endregion

        #region Commands

        #region Evolve command

        private DelegateCommand _evolveCommand;

        public ICommand EvolveCommand
        {
            get { return _evolveCommand ?? (_evolveCommand =
                new DelegateCommand(OnEvolveCommand, _ => true)); }
        }

        private void OnEvolveCommand(object value)
        {
            CurrentAlgorithm.Experiment = CurrentExperiment;
            var res = CurrentAlgorithm.Evolve();
            System.Windows.MessageBox.Show(res.ToString());
        }

        #endregion

        #region Shift Left Command

        private DelegateCommand _shiftLeftCommand;

        public ICommand ShiftLeftCommand
        {
            get { return _shiftLeftCommand ?? (_shiftLeftCommand = 
                new DelegateCommand(OnShiftLeftCommand, CanShiftLeft)); }
        }

        private bool CanShiftLeft(object value)
        {
            return _currentPosition != 0;
        }

        private void OnShiftLeftCommand(object value)
        {
            CurrentExperiment = Experiments[--_currentPosition];
            _shiftLeftCommand.RaiseCanExecuteChanged();
            _shiftRightCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Shift Right Command

        private BenchmarkDepot.Classes.Extensions.DelegateCommand _shiftRightCommand;

        public ICommand ShiftRightCommand
        {
            get
            {
                return _shiftRightCommand ?? (_shiftRightCommand =
                    new BenchmarkDepot.Classes.Extensions.DelegateCommand(OnShiftRightCommand, CanShiftRight));
            }
        }

        private bool CanShiftRight(object value)
        {
            return _currentPosition != Experiments.Count - 1;
        }

        private void OnShiftRightCommand(object value)
        {
            CurrentExperiment = Experiments[++_currentPosition];
            _shiftLeftCommand.RaiseCanExecuteChanged();
            _shiftRightCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #endregion

    }

}


