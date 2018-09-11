using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Osmo.Converters
{
    class DoubleMathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                char mathMethod = parameter.ToString()[0];

                double baseValue = double.Parse(value.ToString());
                double parameterValue = double.Parse(parameter.ToString().Substring(1));
                switch (mathMethod)
                {
                    case '+':
                        return baseValue + parameterValue;
                    case '-':
                        return baseValue - parameterValue;
                    case '*':
                        return baseValue * parameterValue;
                    case '/':
                        return baseValue / parameterValue;
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
