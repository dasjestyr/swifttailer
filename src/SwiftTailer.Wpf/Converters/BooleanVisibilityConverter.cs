using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SwiftTailer.Wpf.Converters
{
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return value;

            var boolVal = (bool) value;
            return boolVal
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
                return value;

            var visibilityVal = (Visibility) value;
            return visibilityVal == Visibility.Visible;
        }
    }
}
