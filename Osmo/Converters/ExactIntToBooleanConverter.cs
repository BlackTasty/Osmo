using Osmo.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Osmo.Converters
{
    class ExactIntToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                var underlyingType = Enum.GetUnderlyingType(value.GetType());
                value = System.Convert.ChangeType(value, underlyingType);
            }
            return Parser.TryParse(value.ToString(), -1) == Parser.TryParse(parameter.ToString(), -1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
