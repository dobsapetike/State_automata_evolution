using System;

namespace BenchmarkDepot.Classes.Core.EAlgotihms.Parameters
{

    /// <summary>
    /// Parameters specific to NEAT evolution
    /// </summary>
    public class NEATParameters
    {

        #region Defaults

        const int DefaultMaxSpecieCount = 25;
        const double DefaultCompatibilityThreshold = 3.0;

        const double DefaultCoefExcessGeneFactor = 1.0;
        const double DefaultCoefDisjointGeneFactor = 1.0;
        const double DefaultCoefMatchinWeightDifferenceFactor = 0.3;

        #endregion

        #region Private fields

        private int _maxSpecieCount;
        private double _compatibilityThreshold;

        private double _coefExcessGeneFactor;
        private double _coefDisjointGeneFactor;
        private double _coefMatchingWeightDifferenceFactor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the maximum number of species
        /// </summary>
        public int MaxSpecieCount
        {
            get { return _maxSpecieCount; }
            set { _maxSpecieCount = value; }
        }

        /// <summary>
        /// Gets and sets the compatibility threshold for speciation
        /// </summary>
        public double CompatibilityThreshold
        {
            get { return _compatibilityThreshold; }
            set { _compatibilityThreshold = value; }
        }

        /// <summary>
        /// Gets and sets the coeficient for setting the importance of the 
        /// number of excess genes in the formula for computing the compatibility distance
        /// </summary>
        public double CoefExcessGeneFactor
        {
            get { return _coefExcessGeneFactor; }
            set { _coefExcessGeneFactor = value; }
        }

        /// <summary>
        /// Gets and sets the coeficient for setting the importance of the 
        /// number of disjoint genes in the formula for computing the compatibility distance
        /// </summary>
        public double CoefDisjointGeneFactor
        {
            get { return _coefDisjointGeneFactor; }
            set { _coefDisjointGeneFactor = value; }
        }

        /// <summary>
        /// Gets and sets the coeficient for setting the importance of the 
        /// weight difference of matching genes in the formula for computing the compatibility distance
        /// </summary>
        public double CoefMatchingWeightDifferenceFactor
        {
            get { return _coefMatchingWeightDifferenceFactor; }
            set { _coefMatchingWeightDifferenceFactor = value; }
        }

        #endregion

        #region Constuctor

        public NEATParameters()
        {
            _maxSpecieCount = DefaultMaxSpecieCount;
            _compatibilityThreshold = DefaultCompatibilityThreshold;

            _coefExcessGeneFactor = DefaultCoefExcessGeneFactor;
            _coefDisjointGeneFactor = DefaultCoefDisjointGeneFactor;
            _coefMatchingWeightDifferenceFactor = DefaultCoefMatchinWeightDifferenceFactor;
        }

        #endregion

    }

}
