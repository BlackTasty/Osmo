using MaterialDesignThemes.Wpf;
using Osmo.Core.Objects;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Osmo.Converters
{
    class FileTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FileType)
            {
                switch ((FileType)value)
                {
                    case FileType.Audio:
                        return PackIconKind.FileMusic;
                    case FileType.Configuration:
                        return PackIconKind.FileXml;
                    case FileType.Image:
                        return PackIconKind.FileImage;
                    default:
                        return PackIconKind.File;
                }
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
