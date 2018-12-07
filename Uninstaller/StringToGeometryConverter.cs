using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Uninstaller
{
    public class StringToGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return ToGeometry(value.ToString());
            else
                return Geometry.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public static Geometry ToGeometry(string value)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(value.ToString()))
                    return Geometry.Parse(value.ToString());
                else
                    return Geometry.Empty;
            }
            catch
            {
                return Geometry.Empty;
            }
        }
    }
}
