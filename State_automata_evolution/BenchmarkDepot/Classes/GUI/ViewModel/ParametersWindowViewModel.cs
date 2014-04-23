using BenchmarkDepot.Classes.Misc;
using BenchmarkDepot.Classes.Core.Experiments;
using BenchmarkDepot.Classes.Core.EAlgotihms;
using BenchmarkDepot.Classes.Core.EAlgotihms.Parameters;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace BenchmarkDepot.Classes.GUI.ViewModel
{

    public class ParametersWindowViewModel : ObservableObject
    {

        #region Properties

        #region Parameters

        public NEATParameters NEATParameters
        {
            get; private set;
        }

        public GeneralEAParameters GeneralParameters
        {
            get; private set;
        }

        public ExperimentProperties ExperimentProperties
        {
            get; private set;
        }

        #endregion

        #region Preset

        /// <summary>
        /// Collection of every available presest
        /// </summary>
        public ObservableCollection<Preset> PresetCollection { get; private set; }

        /// <summary>
        /// Gets and sets the currently selected preset
        /// </summary>
        public Preset CurrentPreset
        {
            get
            {
                return _currentPreset;
            }
            set
            {
                if (_currentPreset == value) return;
                _currentPreset = value;
                _applyPresetCommand.Execute(null);
                RaisePropertyChanged(() => CurrentPreset);
            }
        }

        private Preset _currentPreset;

        #endregion

        #endregion

        #region Constructor

        public ParametersWindowViewModel(NEATAlgorithm neat)
        {
            NEATParameters = neat.NEATParameters;
            GeneralParameters = neat.GeneralEAParameters;
            ExperimentProperties = neat.Experiment.Properties;

            PresetCollection = new ObservableCollection<Preset> { new DefaultNeatPreset() };
            foreach (var preset in Preset.GetExistingPresetNames())
            {
                var p = Preset.LoadFromFile(preset);
                if (p == null) continue;
                PresetCollection.Add(p);
            }
        }

        #endregion

        #region Commands

        #region Apply preset command

        private DelegateCommand _applyPresetCommand;

        /// <summary>
        /// Sets currently selected preset values
        /// </summary>
        public ICommand ApplyPresetCommand
        {
            get
            {
                return _applyPresetCommand ?? (_applyPresetCommand =
                    new DelegateCommand(OnApplyPresetCommand, _ => true));
            }
        }

        private void OnApplyPresetCommand(object value)
        {
            if (CurrentPreset == null) return;
            NEATParameters.LoadFromPreset(CurrentPreset);
            GeneralParameters.LoadFromPreset(CurrentPreset);
        }

        #endregion

        #region Save preset command

        private DelegateCommand _savePresetCommand;

        /// <summary>
        /// Sets currently selected preset values
        /// </summary>
        public ICommand SavePresetCommand
        {
            get
            {
                return _savePresetCommand ?? (_savePresetCommand =
                    new DelegateCommand(OnSavePresetCommand, _ => true));
            }
        }

        private void OnSavePresetCommand(object value)
        {
            var name = value as string;
            if (!Preset.ControlPresetName(name))
            {
                System.Windows.MessageBox.Show("Sorry, but there is something wrong with the preset name you gave, " +
                    "maybe it's already used, try out another one please!", "Invalid preset name");
                return;
            }

            var preset = new Preset(name, NEATParameters, GeneralParameters);
            preset.SaveToFile();
            PresetCollection.Add(preset);
        }

        #endregion

        #endregion

    }

}
