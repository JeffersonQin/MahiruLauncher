using System;
using System.Globalization;
using Avalonia.Data.Converters;
using MahiruLauncher.DataModel;
using Avalonia.Media;

namespace MahiruLauncher.Converter
{
    public class StatusBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ScriptStatus)value switch
            {
                ScriptStatus.Success => Brushes.LightGreen,
                ScriptStatus.Killed => Brushes.Orange,
                ScriptStatus.Running => Brushes.LightBlue,
                ScriptStatus.Error => Brushes.PaleVioletRed,
                ScriptStatus.Waiting => Brushes.White,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}