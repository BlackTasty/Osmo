using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Osmo.Converters
{
    class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var underlyingType = Enum.GetUnderlyingType(value.GetType());
            int test = (int)System.Convert.ChangeType(value, underlyingType);
            return (int)System.Convert.ChangeType(value, underlyingType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
