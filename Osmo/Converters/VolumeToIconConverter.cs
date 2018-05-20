using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Osmo.Converters
{
    class VolumeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double volume)
            {
                if (volume > .8)
                    return PackIconKind.VolumeHigh;
                else if (volume > .5)
                    return PackIconKind.VolumeMedium;
                else if (volume > .2)
                    return PackIconKind.VolumeLow;
                else
                    return PackIconKind.VolumeOff;
            }
            else
                return PackIconKind.VolumeHigh;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
