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
            var category = highlighter.Category.ToString();

            string colorValue = null;
            switch (category)
            {
                case "Find":
                    colorValue = "Green";
                    break;
            }

            return colorValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class PinningConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return value;

            var val = (bool) value;
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
