using System;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    internal abstract class Mosquito
    {
        public abstract int X { get; protected set; }
        public abstract int Y { get; protected set; }
        public abstract bool IsDead { get; protected set; }
        public abstract int AreaSize { get; protected set; }
        protected int _stepLeft;
        public virtual void TakeStep() 
        { 
            if (--_stepLeft <= 0) IsDead = true; 
        }
    }

    internal class SteadyMosquito : Mosquito
    {
        public override int X { get; protected set; }
        public override int Y { get; protected set; }
        public override bool IsDead { get; protected set; }
        public override int AreaSize { get; protected set; }
        public SteadyMosquito(int x, int y, int area, int steps)
        {
            X = x;
            Y = y;
            AreaSize = area;
            _stepLeft = steps;
        }
        public override void TakeStep()
        {
            // don't you ever move
            base.TakeStep();
        }
    }

    internal class HorizontalMosquito : Mosquito
    {
        public override int X { get; protected set; }
        public override int Y { get; protected set; }
        public override bool IsDead { get; protected set; }
        public override int AreaSize { get; protected set; }
        public HorizontalMosquito(int x, int y, int area, int steps)
        {
            X = x;
            Y = y;
            AreaSize = area;
            _stepLeft = steps;
        }
        private bool leftdir = true;
        public override void TakeStep()
        {
            base.TakeStep();
            if (leftdir)
            {
                if (X > 0) --X;
                else
                {
                    ++X;
                    leftdir = false;
                }
            }
            else 
            {
                if (X < AreaSize - 1) ++X;
                else
                {
                    --X;
                    leftdir = true;
                }
            }
        }
    }

    internal class VerticalMosquito : Mosquito
    {
        public override int X { get; protected set; }
        public override int Y { get; protected set; }
        public override bool IsDead { get; protected set; }
        public override int AreaSize { get; protected set; }
        public VerticalMosquito(int x, int y, int area, int steps)
        {
            X = x;
            Y = y;
            AreaSize = area;
            _stepLeft = steps;
        }
        private bool updir = true;
        public override void TakeStep()
        {
            base.TakeStep();
            if (updir)
            {
                if (Y > 0) --Y;
                else
                {
                    ++Y;
                    updir = false;
                }
            }
            else
            {
                if (Y < AreaSize - 1) ++Y;
                else
                {
                    --Y;
                    updir = true;
                }
            }
        }
    }

    public class CatchMeIfYouCanExperiment : IExperiment
    {

        private Random random = new Random();
        private ExperimentProperties _properties;

        public string Name
        {
            get { return "Catch me if you can"; }
        }

        public ExperimentProperties Properties
        {
            get { return _properties; }
        }

        public string Description
        {
            get { return "Evolves an automaton, which controls the movement of a hungry frog in a swamp "
                + "of size NxN, chasing mosquitos for dinner, whilst avoiding the deadly stork, which also " 
                + "roams the swamps."; }
        }

        private double _requiredFitness;
        public double RequiredFitness
        {
            get { return _requiredFitness; }
        }

        private TransitionTrigger[] _events = new[]
        {
            new TransitionTrigger("Food left"),
            new TransitionTrigger("Food right"),
            new TransitionTrigger("Food up"),
            new TransitionTrigger("Food down"),
            new TransitionTrigger("Stork arrives"),
            new TransitionTrigger("Stork leaves"),
        };

        public IEnumerable<TransitionTrigger> TransitionEvents
        {
            get { return _events; }
        }

        public IEnumerable<string> TransitionTranslations
        {
            get { return new[] {  "X" }; }
        }

        public IEnumerable<Action> TransitionActions
        {
            get 
            { 
                return new[]
                {
                    new Action(StepLeft),
                    new Action(StepRight),
                    new Action(StepUp),
                    new Action(StepDown),
                    new Action(DoNothing),
                }; 
            }
        }

        public CatchMeIfYouCanExperiment()
        {
            _properties = new ExperimentProperties();
            _properties.AddProperty("Grid size", 5);
            _properties.AddProperty("Frog steps", 1);
            _properties.AddProperty("Mosquito life", 10);
            _properties.AddProperty("Missable mosquito count", 3);
            _properties.AddProperty("Stork chance", 3);
            _properties.AddProperty("Stork time", 2);
            _properties.AddProperty("Required fitness", 5000);
        }

        public void Reset()
        {
            _mosquitoesMissedCount = 0;
            _mosquitesMissedMax = (int)_properties["Missable mosquito count"];
            _gridSize = (int)_properties["Grid size"];
            _frogSteps = (int)_properties["Frog steps"];
            _mosquitoLife = (int)_properties["Mosquito life"];
            _storkChance = _properties["Stork chance"] / 100d;
            _storkTime = (int)_properties["Stork time"];
            
            _frogX = _frogY = _gridSize / 2;
            _storkPresent = false;
            _requiredFitness = _properties["Required fitness"];

            CreateNewMosquito();
        }

        #region Experiment variables and methods

        private int _gridSize;
        private int _storkTime, _storkTimeLeft;
        private double _storkChance;
        private bool _storkPresent;
        private int _frogX, _frogY;
        private int _frogSteps;
        private int _mosquitoLife;
        private int _mosquitoesMissedCount, _mosquitesMissedMax;
        private Mosquito _mosquito;

        private void DoNothing() { }
        private void StepLeft() { if (_frogX > 0) --_frogX; }
        private void StepRight() { if (_frogX < _gridSize-1) ++_frogX; }
        private void StepDown() { if (_frogY < _gridSize-1) ++_frogY; }
        private void StepUp() { if (_frogY > 0) --_frogY; }

        private void CreateNewMosquito()
        {
            var type = random.Next(3);
            int x = random.Next(_gridSize), y = random.Next(_gridSize);
            switch (type)
            {
                case 0: _mosquito = new SteadyMosquito(x, y, _gridSize, _mosquitoLife); break;
                case 1: _mosquito = new HorizontalMosquito(x, y, _gridSize, _mosquitoLife); break;
                case 2: _mosquito = new VerticalMosquito(x, y, _gridSize, _mosquitoLife); break;
            }
        }

        private bool PerformOneStep(Transducer t)
        {
            if (_storkTimeLeft > 0 && --_storkTimeLeft == 0)
            {
                _storkPresent = false;
                t.ShiftState(_events[5]);
            }
            else if (!_storkPresent && _storkChance > random.NextDouble())
            {
                _storkPresent = true;
                _storkTimeLeft = _storkTime; 
                t.ShiftState(_events[4]);
            }

            int x = _frogX, y = _frogY;
            for (var i = 0; i < _frogSteps; ++i)
            {
                bool horiz = Math.Abs(_frogX - _mosquito.X) > Math.Abs(_frogY - _mosquito.Y);

                if (horiz)
                {
                    if (_mosquito.X < _frogX)
                        t.ShiftState(_events[0]);
                    else
                        t.ShiftState(_events[1]);
                }
                else
                {
                    if (_mosquito.Y < _frogY)
                        t.ShiftState(_events[2]);
                    else
                        t.ShiftState(_events[3]);
                }
                if (_frogX == _mosquito.X && _frogY == _mosquito.Y)
                {
                    CreateNewMosquito();
                    return true;
                }
            }
            if (_storkPresent && (x != _frogX || y != _frogY))
            {
                _mosquitoesMissedCount = _mosquitesMissedMax;
                return false;
            }

            _mosquito.TakeStep();
            if (_frogX == _mosquito.X && _frogY == _mosquito.Y)
            {
                CreateNewMosquito();
                return true;
            }

            if (_mosquito.IsDead)
            {
                CreateNewMosquito();
                _mosquitoesMissedCount++;
            }
            return false;
        }

        #endregion

        #region Public methods

        public double Run(Transducer transducer)
        {
            transducer.Reset();
            Reset();

            var score = 0d;
            for (;;)
            {
                var ok = PerformOneStep(transducer);
                if (_mosquitoesMissedCount == _mosquitesMissedMax) break;

                if (ok) score++;
                if (score >= RequiredFitness) 
                    break;
            }

            return score;
        }

        public void TestDrive(Transducer transducer)
        {
            transducer.Reset();
            Reset();

            for (;;)
            {

                PerformOneStep(transducer);

                Console.Out.WriteLine(string.Format("Stork present: {0}", _storkPresent));
                for (var i = 0; i < _gridSize; ++i)
                {
                    for (var j = 0; j < _gridSize; ++j)
                    {
                        if (_frogX == j && _frogY == i) Console.Out.Write("F");
                        else if (_mosquito.X == j && _mosquito.Y == i) Console.Out.Write("M");
                        else Console.Out.Write(".");
                    }
                    Console.WriteLine();
                }
                Console.Out.WriteLine();

                var s = Console.In.ReadLine();
                if (s.ToLower() == "exit") break;
            }
        }

        #endregion

    }

}
