using System;
using System.Globalization;
using System.Windows.Data;
using SwiftTailer.Wpf.Filters;

namespace SwiftTailer.Wpf.Converters
{
    public class HighlightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var highlighter = (LogHighlight) value;
            return highlighter.Category.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
