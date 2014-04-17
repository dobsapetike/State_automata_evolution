namespace BenchmarkDepot.Classes.Core.EAlgotihms.Parameters
{

    /// <summary>
    /// A parameter preset for NEAT evolution
    /// </summary>
    class DefaultNeatPreset : Preset
    {

        public DefaultNeatPreset() : base("Default NEAT preset")
        {
            SpeciesAllowed = true;
            CriticalSpecieCount = 35;
            AllowedSpeciesStagnatedGenerationCount = 3;
            CompatibilityThreshold = 0.5;
            MinCompatibilityThreshold = 0.1;
            CompatibilityThresholdDelta = 0.02;
            CoefExcessGeneFactor = 1d;
            CoefDisjointGeneFactor = 1d;
            CoefMatchingWeightDifferenceFactor = 1d;
            MatchingWeightDifferenceValue = 1d;
            AddNodeMutationProbability = 0.4;
            AddTransitionMutationProbability = 0.85;
            SurvivalRate = 0.7;
            InnovationResetPerGeneration = false;

            InitialPopulationSize = 1500;
            MaxPopulationSize = 1500;
            MaxIndividualSize = 10;
            GenerationThreshold = 2000;
            SelectionProportion = 1d;
            ReplacementProportion = 0.85;
            CrossoverProbability = 0.5;
            StateDeletionMutationProbability = 0d;
            TransitionDeletionMutationProbability = 0d;
            TransitionActionMutationProbability = 0.85;
            TransitionTranslationMutationProbability = 0.85;
            TransitionTriggerMutationProbability = 0.85;
        }

    }

}
