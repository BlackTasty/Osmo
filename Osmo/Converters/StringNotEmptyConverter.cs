using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Osmo.Converters
{
    class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                if (value.GetType() == typeof(string))
                {
                    return !string.IsNullOrWhiteSpace((value ?? "").ToString());
                }
                else return true;
            }
            else
            {
                if (value.GetType() == typeof(string))
                {
                    return !string.IsNullOrWhiteSpace((value ?? "").ToString()) ? Visibility.Visible : Visibility.Collapsed;
                }
                else return Visibility.Collapsed;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
