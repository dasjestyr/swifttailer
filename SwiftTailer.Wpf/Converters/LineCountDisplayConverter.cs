using System;
using System.Globalization;
using System.Windows.Data;

namespace SwiftTailer.Wpf.Converters
{
    public class LineCountDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return value;
            var count = (int)value;
            return $"Displaying {count} lines ({Settings.MaxDisplayLogLines} max)";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
