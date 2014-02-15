﻿using System;
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

        private byte _currentPosition = 0;

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
            var e = new Evol("evolution");
            CurrentAlgorithm = e;
            Algorithms = new ObservableCollection<IEvolutionaryAlgorithm>
                {
                    new Evol("petike"),
                    new Evol("another"),
                    e
                };
            var exp = new Exp("exp", "this is a very accurate description");
            CurrentExperiment = exp;
            Experiments = new ObservableCollection<IExperiment>
            {
                exp,
                new Exp("dalsi", "hahaha"),
                new Exp("experiment", "experiment description"),
                new Exp("haha", "hihi")
            };

            Exp exper = new Exp("test","test");
            exper.TransitionActions = new List<Action> { null };
            exper.TransitionEvents = new List<string> { "enemy close", "enemy far", "danger" };
            exper.TransitionTranslations = new List<string> { "peti", "dobsa", "a", "b", "c" };

            //TransducerState a = new TransducerState(1);
            //var b = new TransducerState(2);
            //Transducer t = new Transducer();
            //t.AddState(a); t.AddState(b);
            //TransducerTransition tt = new TransducerTransition(null, "peti", 1);
            ////t.AddTransition(a, b, "danger", tt);

            //NEATAlgorithm neat = new NEATAlgorithm();
            //neat.Experiment = exper;

            //var g = neat.MutateTransducer(t);
            //var h = neat.MutateTransducer(g);
            //for (int i = 0; i < 10; ++i)
            //    h = neat.MutateTransducer(h);
            //System.Windows.MessageBox.Show("g:\n" + g.ToString() + "\nh:\n" + h.ToString());
        }

        #endregion

        #region Commands

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