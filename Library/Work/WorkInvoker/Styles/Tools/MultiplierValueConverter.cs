using System;
using System.Globalization;

using Xamarin.Forms;

namespace WorkInvoker.Styles.Tools
{
    public class MultiplierValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => GetParameter(value) * GetParameter(parameter);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => GetParameter(value) / GetParameter(parameter);

        private double GetParameter(object parameter)
        {
            if (parameter is double @double)
                return @double;
            else if (parameter is int @int)
                return @int;
            else if (parameter is string @string)
                return double.Parse(@string, NumberStyles.Any, CultureInfo.InvariantCulture);
            else if (parameter is OnPlatform<double> platform)
                return platform;
            return 1;
        }
    }
}
