using Osmo.Core;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Osmo.Converters
{
    public class IntToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Parser.TryParse(value.ToString(), -1) >= Parser.TryParse(parameter.ToString(), -1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
