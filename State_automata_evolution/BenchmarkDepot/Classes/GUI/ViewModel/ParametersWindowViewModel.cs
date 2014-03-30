using BenchmarkDepot.Classes.Core.EAlgotihms.Parameters;

namespace BenchmarkDepot.Classes.GUI.ViewModel
{

    public class ParametersWindowViewModel
    {

        public NEATParameters NEATParameters
        {
            get; private set;
        }

        public GeneralEAParameters GeneralParameters
        {
            get; private set;
        }

        public ParametersWindowViewModel(NEATParameters np, GeneralEAParameters gp)
        {
            NEATParameters = np;
            GeneralParameters = gp;
        }

    }

}
