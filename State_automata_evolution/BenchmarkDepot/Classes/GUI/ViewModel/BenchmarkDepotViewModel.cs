using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using BenchmarkDepot.Classes.Core;
using BenchmarkDepot.Classes.Misc;
using BenchmarkDepot.Classes.GUI;
using BenchmarkDepot.Classes.Core.EAlgotihms;
using BenchmarkDepot.Classes.Core.Experiments;

namespace BenchmarkDepot.Classes.GUI.ViewModel
{

    public class BenchmarkDepotViewModel : ObservableObject
    {

        #region Private fields

        private short _currentPosition = 0;

        #endregion

        #region Properties

        #region Algorithms

        public ObservableCollection<NEATAlgorithm> Algorithms { get; private set; }

        private NEATAlgorithm _currentAlgorithm;

        public NEATAlgorithm CurrentAlgorithm
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
            Algorithms = new ObservableCollection<NEATAlgorithm>
                {
                    new NEATAlgorithm(),
                    new NEATAlgorithmInitialRandom(),
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
            var evolWin = new EvolutionWindow(new EvolutionWindowViewModel(CurrentAlgorithm));
            evolWin.ShowDialog();
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

        private DelegateCommand _shiftRightCommand;

        public ICommand ShiftRightCommand
        {
            get
            {
                return _shiftRightCommand ?? (_shiftRightCommand =
                    new DelegateCommand(OnShiftRightCommand, CanShiftRight));
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


