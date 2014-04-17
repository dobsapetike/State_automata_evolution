using System;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    public class CatchMeIfYouCanExperiment : IExperiment
    {

        public string Name
        {
            get { return "Catch Me if You Can"; }
        }

        public string Description
        {
            get { return "Evolves an automat, which controls the movement of a frog in a swamp "
                + "of size NxN, avoiding the deadly stork, which also roams the swamp."; }
        }

        public double RequiredFitness
        {
            get { return 100d; }
        }

        public IEnumerable<TransitionTrigger> TransitionEvents
        {
            get 
            { 
                return new List<TransitionTrigger> {
                    new TransitionTrigger("Danger Left"),//, true)  { MinParameterValue=0, MaxParameterValue=GridSize},
                    new TransitionTrigger("Danger Right"),//, true) { MinParameterValue=0, MaxParameterValue=GridSize},
                    new TransitionTrigger("Danger Up"),//, true)    { MinParameterValue=0, MaxParameterValue=GridSize},
                    new TransitionTrigger("Danger Down"),//, true)  { MinParameterValue=0, MaxParameterValue=GridSize},
                }; 
            }
        }

        public IEnumerable<string> TransitionTranslations
        {
            get { return new List<string> { "" }; }
        }

        public IEnumerable<Action> TransitionActions
        {
            get 
            {
                return new List<Action>
                {
                    new Action(StepLeft),
                    new Action(StepRight),
                    new Action(StepUp),
                    new Action(StepDown),
                };
            }
        }

        private bool PerformOneStep(Transducer t)
        {
            MoveStork();
            if (_xFrog == _xStork && _yFrog == _yStork) return false;

            for (var i = 0; i < FrogMoves; ++i)
            {
                bool horiz = Math.Abs(_xFrog - _xStork) < Math.Abs(_yStork - _yFrog);

                if (horiz)
                {
                    if (_xStork < _xFrog)
                        t.ShiftState(new TransitionTrigger("Danger Left"));//, _xFrog - _xStork);
                    else
                        t.ShiftState(new TransitionTrigger("Danger Right"));//, _xStork - _xFrog);
                }
                else
                {
                    if (_yStork < _yFrog)
                        t.ShiftState(new TransitionTrigger("Danger Up"));//, _yFrog - _yStork);
                    else
                        t.ShiftState(new TransitionTrigger("Danger Down"));//, _yStork - _yFrog);
                }
                if (_xFrog == _xStork && _yFrog == _yStork) return false;
            }
            return true;
        }

        public double Run(Transducer transducer)
        {
            transducer.Reset();
            _xFrog = _yFrog = 0;
            _xStork = _yStork = GridSize - 1;

            var score = 0d;

            for (;;)
            {
                var ok = PerformOneStep(transducer);
                if (!ok) break;

                if (_xFrog == _xStork && _yFrog == _yStork) break;
                if (++score == RequiredFitness) break;
            }

            return score;
        }

        #region Experiment methods

        private const int GridSize = 8;
        private const int FrogMoves = 3;
        private int _xFrog = 0, _yFrog = 0;
        private int _xStork = GridSize-1, _yStork = GridSize-1;

        private Random r = new Random();

        private void StepLeft()  { if (_xFrog != 0) _xFrog -= 1; }
        private void StepRight() { if (_xFrog != GridSize-1) _xFrog += 1; }
        private void StepUp()    { if (_yFrog != 0) _yFrog -= 1; }
        private void StepDown()  { if (_yFrog != GridSize-1) _yFrog += 1; }
        private void MoveStork()
        {
            bool horiz = Math.Abs(_xFrog - _xStork) > Math.Abs(_yStork - _yFrog);

            //if (horiz)
            //{
                //_xStork += _xFrog > _xStork ? 1 : -1;
            //} else
                //_yStork += _yFrog > _yStork ? 1 : -1;
            if (r.Next(2) == 0 && _xStork != _xFrog) _xStork += _xFrog > _xStork ? 1 : -1;
            if (r.Next(2) == 0 && _yStork != _yFrog) _yStork += _yFrog > _yStork ? 1 : -1;
            //_xStork += r.Next(2) == 0 ? 1 : -1;
            //_yStork += r.Next(2) == 0 ? 1 : -1;

            if (_xStork < 0) _xStork = 0;
            else if (_xStork >= GridSize - 1) _xStork = GridSize - 1;
            else if (_yStork < 0) _yStork = 0;
            else if (_yStork >= GridSize - 1) _yStork = GridSize - 1;
        }

        #endregion

        private void DrawGrid()
        {
            for(var i = 0; i < GridSize; ++i)
            {
                for (var j = 0; j < GridSize; ++j)
                {
                    if (_xStork == j && _yStork == i) Console.Out.Write('S');
                    else if (_xFrog == j && _yFrog == i) Console.Out.Write('F');
                    else Console.Out.Write('.');
                }
                Console.Out.WriteLine();
            }
        }

        public void TestDrive(Transducer transducer)
        {
            transducer.Reset();
            _xFrog = _yFrog = 0;
            _xStork = _yStork = GridSize - 1;

            for (;;)
            {
                DrawGrid();

                var s = Console.ReadLine();
                if (s.ToLower() == "exit") break;

                var ok = PerformOneStep(transducer);
                if (!ok) break;
            }
        }

    }

}
