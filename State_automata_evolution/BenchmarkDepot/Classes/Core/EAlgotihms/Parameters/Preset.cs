using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace BenchmarkDepot.Classes.Core.EAlgotihms.Parameters
{

    /// <summary>
    /// Represents a set of parameter setting for the evolutionary algorithm
    /// </summary>
    [Serializable]
    public class Preset
    {

        private const string PresetPath = "Presets/";

        #region Properties

        public string Name { get; private set; }

        #region Neat params

        public bool SpeciesAllowed { get; set; }
        public int CriticalSpecieCount { get; set; }
        public int AllowedSpeciesStagnatedGenerationCount { get; set; }
        public double CompatibilityThreshold { get; set; }
        public double MinCompatibilityThreshold { get; set; }
        public double CompatibilityThresholdDelta { get; set; }
        public double CoefExcessGeneFactor { get; set; }
        public double CoefDisjointGeneFactor { get; set; }
        public double CoefMatchingWeightDifferenceFactor { get; set; }
        public double MatchingWeightDifferenceValue { get; set; }
        public double AddNodeMutationProbability { get; set; }
        public double AddTransitionMutationProbability { get; set; }
        public double SurvivalRate { get; set; }
        public bool InnovationResetPerGeneration { get; set; }

        #endregion

        #region General params

        public int InitialPopulationSize { get; set; }
        public int MaxPopulationSize { get; set; }
        public int MaxIndividualSize { get; set; }
        public int GenerationThreshold { get; set; }
        public double SelectionProportion { get; set; }
        public double ReplacementProportion { get; set; }
        public double CrossoverProbability { get; set; }
        public double StateDeletionMutationProbability { get; set; }
        public double TransitionDeletionMutationProbability { get; set; }
        public double TransitionActionMutationProbability { get; set; }
        public double TransitionTranslationMutationProbability { get; set; }
        public double TransitionTriggerMutationProbability { get; set; }

        #endregion

        #endregion

        #region Constructor

        public Preset(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes values from parameter classes
        /// </summary>
        public Preset(string name, NEATParameters np, GeneralEAParameters gp)
        {
            Name = name;

            SpeciesAllowed = np.SpeciesAllowed;
            CriticalSpecieCount = np.CriticalSpecieCount;
            AllowedSpeciesStagnatedGenerationCount = np.AllowedSpeciesStagnatedGenerationCount;
            CompatibilityThreshold = np.CompatibilityThreshold;
            MinCompatibilityThreshold = np.MinCompatibilityThreshold;
            CompatibilityThresholdDelta = np.CompatibilityThresholdDelta;
            CoefExcessGeneFactor = np.CoefExcessGeneFactor;
            CoefDisjointGeneFactor = np.CoefDisjointGeneFactor;
            CoefMatchingWeightDifferenceFactor = np.CoefMatchingWeightDifferenceFactor;
            MatchingWeightDifferenceValue = np.MatchingWeightDifferenceValue;
            AddNodeMutationProbability = np.AddNodeMutationProbability;
            AddTransitionMutationProbability = np.AddTransitionMutationProbability;
            SurvivalRate = np.SurvivalRate;
            InnovationResetPerGeneration = np.InnovationResetPerGeneration;

            InitialPopulationSize = gp.InitialPopulationSize;
            MaxPopulationSize = gp.MaxPopulationSize;
            MaxIndividualSize = gp.MaxIndividualSize;
            GenerationThreshold = gp.GenerationThreshold;
            SelectionProportion = gp.SelectionProportion;
            ReplacementProportion = gp.ReplacementProportion;
            CrossoverProbability = gp.CrossoverProbability;
            StateDeletionMutationProbability = gp.StateDeletionMutationProbability;
            TransitionDeletionMutationProbability = gp.TransitionDeletionMutationProbability;
            TransitionActionMutationProbability = gp.TransitionActionMutationProbability;
            TransitionTranslationMutationProbability = gp.TransitionTranslationMutationProbability;
            TransitionTriggerMutationProbability = gp.TransitionTriggerMutationProbability;
        }

        #endregion

        #region Methods

        #region Serialization

        /// <summary>
        /// Turns the object into a stream of bytes via serialization
        /// </summary>
        public void SaveToFile()
        {
            if (!Directory.Exists(PresetPath)) Directory.CreateDirectory(PresetPath);

            var path = PresetPath + Name + ".preset";
            var fs = new FileStream(path, FileMode.Create);
            var bf = new BinaryFormatter();
            bf.Serialize(fs, this);
            fs.Close();
        }

        /// <summary>
        /// Deserializes a Preset object from file
        /// </summary>
        public static Preset LoadFromFile(string presetName)
        {
            var filePath = PresetPath + presetName + ".preset";
            if (!File.Exists(filePath)) return null;
            
            var fs = File.Open(filePath, FileMode.Open);
            var bf = new BinaryFormatter();
            var result = (Preset) bf.Deserialize(fs);
            fs.Close();
            return result;
        }

        #endregion

        /// <summary>
        /// Returns an array of every available preset names
        /// </summary>
        public static string[] GetExistingPresetNames()
        {
            if (!Directory.Exists(PresetPath)) return new string[0];
            return Directory.GetFiles(PresetPath, "*.preset")
                .Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
        }

        /// <summary>
        /// Simple control of a preset name - returns true if name is not taken
        /// </summary>
        public static bool ControlPresetName(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            if (GetExistingPresetNames().Contains(name)) return false;
            return true;
        }

        #endregion

    }

}
