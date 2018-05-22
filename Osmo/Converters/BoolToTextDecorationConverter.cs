using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace Osmo.Converters
{
    class BoolToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (File.Exists(value.ToString()))
                return TextDecorations.Strikethrough;
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
