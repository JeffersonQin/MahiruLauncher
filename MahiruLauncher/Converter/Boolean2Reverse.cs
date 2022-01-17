using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MahiruLauncher.Converter
{
    public class Boolean2Reverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
         => !(bool?)value ?? true;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
         => !(value as bool?);
    }
}
