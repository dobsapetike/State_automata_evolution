using System;
using System.Windows;
using System.Windows.Data;

namespace BenchmarkDepot.Classes.Misc
{

    /// <summary>
    /// Simple converter for converting a bool to Visibility value
    /// Optional parameter can change the mapping
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (bool) value;
            var param = parameter as string;
            if (param == "reverse") val = !val;
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
