using BenchmarkDepot.Classes.Misc;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace BenchmarkDepot.Classes.Core.Experiments
{

    /// <summary>
    /// .NET dictionary doesn't support two-way bindings so instead experiments
    /// will use a this wrapper class
    /// </summary>
    public class ExperimentProperties
    {

        public ObservableCollection<ExperimentProperty> Properties { get; private set; }

        public ExperimentProperties()
        {
            Properties = new ObservableCollection<ExperimentProperty>();
        }

        public void AddProperty(string name, double value)
        {
            Properties.Add(new ExperimentProperty(name, value));
        }

        public double this[string prop]
        {
            get
            {
                var property = Properties.FirstOrDefault(x => x.Name == prop); 
                if (property == null) throw new ArgumentException(prop);
                return property.Value;
            }
            set
            {
                var property = Properties.FirstOrDefault(x => x.Name == prop);
                if (property == null) throw new ArgumentException(prop);
                property.Value = value;
            }
        } 
    }

    public class ExperimentProperty
    {

        public string Name { get; set; }
        public double Value { get; set; }

        public ExperimentProperty(string name, double value)
        {
            Name = name;
            Value = value;
        }

    }

}
