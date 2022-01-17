using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using MahiruLauncher.DataModel;

namespace MahiruLauncher.Converter
{
    public class StatusBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (ScriptStatus)value switch
            {
                ScriptStatus.Success => Brushes.Green,
                ScriptStatus.Killed => Brushes.DarkOrange,
                ScriptStatus.Running => Brushes.MediumBlue,
                ScriptStatus.Error => Brushes.Red,
                ScriptStatus.Waiting => Brushes.LightGray,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}