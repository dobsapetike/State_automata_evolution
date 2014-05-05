using System;
using System.Diagnostics;
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
                new NEATAlgorithmCompleteStructure(),
            };
            CurrentAlgorithm = Algorithms[0];

            Experiments = new ObservableCollection<IExperiment>
            {
                new BinaryTransducerExperiment(),
                new NoZeroLeftBehindExperiment(),
                new TermostatExperiment(),
                new DressingRoomExperiment(),
                new CatchMeIfYouCanExperiment(),
            };
            CurrentExperiment = Experiments[0];

            // initialize console window
            ConsoleManager.AllocConsole();
            ConsoleManager.HideConsole();

            //var ex = new CatchMeIfYouCanExperiment();

            //var t = new Transducer();
            //var s = new TransducerState(1);
            //var s2 = new TransducerState(2);
            //t.AddState(s); t.AddState(s2);
            //t.AddTransition(s, s, ex.TransitionEvents.ElementAt(0),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(0), "X", 1));
            ////t.AddTransition(s, s, ex.TransitionEvents.ElementAt(1),
            ////    new TransducerTransition(ex.TransitionActions.ElementAt(1), "X", 1));
            //t.AddTransition(s, s, ex.TransitionEvents.ElementAt(2),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(2), "X", 1));
            //t.AddTransition(s, s, ex.TransitionEvents.ElementAt(3),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(3), "X", 1));
            //t.AddTransition(s, s2, ex.TransitionEvents.ElementAt(4),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(4), "X", 2));
            //t.AddTransition(s2, s, ex.TransitionEvents.ElementAt(5),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(4), "X", 3));
            //var t2 = new Transducer();
            //var s21 = new TransducerState(1);
            //var s22 = new TransducerState(2);
            //t2.AddState(s21); t2.AddState(s22);
            //t2.AddTransition(s, s, ex.TransitionEvents.ElementAt(0),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(0), "X", 1));
            //t2.AddTransition(s21, s21, ex.TransitionEvents.ElementAt(1),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(1), "X", 1));
            //t2.AddTransition(s21, s21, ex.TransitionEvents.ElementAt(2),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(2), "X", 1));
            //t2.AddTransition(s21, s21, ex.TransitionEvents.ElementAt(3),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(3), "X", 1));
            //t2.AddTransition(s21, s22, ex.TransitionEvents.ElementAt(4),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(4), "X", 2));
            //t2.AddTransition(s22, s21, ex.TransitionEvents.ElementAt(5),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(0), "X", 3));
            //var t3 = new Transducer();
            //var s31 = new TransducerState(1);
            //var s32 = new TransducerState(2);
            //t3.AddState(s); t3.AddState(s2);
            //t3.AddTransition(s31, s31, ex.TransitionEvents.ElementAt(0),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(0), "X", 1));
            //t3.AddTransition(s31, s31, ex.TransitionEvents.ElementAt(1),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(1), "X", 1));
            //t3.AddTransition(s31, s31, ex.TransitionEvents.ElementAt(2),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(2), "X", 1));
            //t3.AddTransition(s31, s31, ex.TransitionEvents.ElementAt(3),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(3), "X", 1));
            //t3.AddTransition(s31, s32, ex.TransitionEvents.ElementAt(4),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(4), "X", 2));
            //t3.AddTransition(s32, s31, ex.TransitionEvents.ElementAt(5),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(0), "X", 3));
            //var np = new BenchmarkDepot.Classes.Core.EAlgotihms.Parameters.NEATParameters();
            //var gp = new BenchmarkDepot.Classes.Core.EAlgotihms.Parameters.GeneralEAParameters();
            //var sp = new BenchmarkDepot.Classes.Core.EAlgotihms.Accessories.Species(0, t, np, gp);
            //sp.InsertNew(t2); sp.InsertNew(t3);
            //sp.SelectNewRepresentative();
            ////System.Windows.MessageBox.Show(t + "");
            ////System.Windows.MessageBox.Show(t2 + "");
            //System.Windows.MessageBox.Show(sp.Representative + "");
            
            //var t = new Transducer();
            //var s = new TransducerState(1);
            //var s2 = new TransducerState(2);
            //t.AddState(s); t.AddState(s2);
            //t.AddTransition(s, s, ex.TransitionEvents.ElementAt(0),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(0), "X", 1));
            //t.AddTransition(s, s, ex.TransitionEvents.ElementAt(1),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(1), "X", 1));
            //t.AddTransition(s, s, ex.TransitionEvents.ElementAt(2),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(2), "X", 1));
            //t.AddTransition(s, s, ex.TransitionEvents.ElementAt(3),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(3), "X", 1));
            //t.AddTransition(s, s2, ex.TransitionEvents.ElementAt(4),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(4), "X", 2));
            //t.AddTransition(s2, s, ex.TransitionEvents.ElementAt(5),
            //    new TransducerTransition(ex.TransitionActions.ElementAt(4), "X", 3));
            //var sc = ex.Run(t);
            //var tc = sc;
            //ex.TestDrive(t);


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


