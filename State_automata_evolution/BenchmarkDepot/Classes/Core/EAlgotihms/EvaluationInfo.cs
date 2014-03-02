using System;

namespace BenchmarkDepot.Classes.Core.EAlgotihms
{

    /// <summary>
    /// Wrapper class containing every information relevant to 
    /// transducer evaluation
    /// </summary>
    public class EvaluationInfo
    {

        #region Private fields

        /// <summary>
        /// Fitness value - represents the quality of the transducer 
        /// </summary>
        private double _fitness;

        /// <summary>
        /// Adjusted fitness value - explicitly shared fitness of the specie 
        /// </summary>
        private double _adjustedFitness;

        /// <summary>
        /// Number of evalutations
        /// </summary>
        private int _evaluationCount;

        /// <summary>
        /// Number of skipped evaluations - eg. when no structural modifications occurred  
        /// </summary>
        private int _evalutationSkipCount;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the fitness value
        /// </summary>
        public double Fitness
        {
            get { return _fitness; }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentException("Fitness value must be a positive number!");
                }
                _fitness = value;
                _evaluationCount++;
            }
        }

        /// <summary>
        /// Gets and sets the adjusted fitness value
        /// </summary>
        public double AdjustedFitness
        {
            get { return _adjustedFitness; }
            set
            {
                if (value < 0.0)
                {
                    throw new ArgumentException("Fitness value must be a positive number!");
                }
                _adjustedFitness = value;
            }
        }

        /// <summary>
        /// Returns whether the transducer has been evaluated
        /// </summary>
        public bool IsEvaluated
        {
            get { return _evaluationCount != 0 || _evalutationSkipCount != 0; }
        }

        /// <summary>
        /// Gets the number of evaluations
        /// </summary>
        public int EvaluationCount
        {
            get { return _evaluationCount; }
        }

        /// <summary>
        /// Gets the number of skipped evaluations
        /// </summary>
        public int SkippedEvaluationCount
        {
            get { return _evalutationSkipCount; }
        }

        /// <summary>
        /// Gets the age of the transducer
        /// </summary>
        public int Age
        {
            get { return _evaluationCount + _evalutationSkipCount; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Called when the evaluation of the transducer is redundant. 
        /// Increases the skip count.
        /// </summary>
        public void SkipEvaluation()
        {
            ++_evalutationSkipCount;
        }

        #endregion

    }

}
