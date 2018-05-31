using System;
using System.Globalization;
using System.Windows.Data;

namespace Osmo.Converters
{
    class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return !string.IsNullOrWhiteSpace((value ?? "").ToString());
            }
            else return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
