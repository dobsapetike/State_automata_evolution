using System;
using System.Collections.Generic;
using BenchmarkDepot.Classes.Core;

namespace BenchmarkDepot.Classes.Core.Interfaces
{

    public interface IExperiment
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<string> TransitionEvents { get; }
        IEnumerable<string> TransitionTranslations { get; }
        IEnumerable<Action> TransitionActions { get; }
        int Run(Transducer transducer);
    }

}
