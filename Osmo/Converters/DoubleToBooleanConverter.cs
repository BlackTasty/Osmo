using Osmo.Core;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Osmo.Converters
{
    class DoubleToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Parser.TryParse(value.ToString().Replace('.', ','), 0.0) >=
                Parser.TryParse(parameter.ToString().Replace('.', ','), 0.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
