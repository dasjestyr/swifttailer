using System;
using System.Windows;
using System.Windows.Data;

namespace SwiftTailer.Wpf.Converters
{
    class EnumBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            var parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var parameterString = parameter as string;
            return parameterString == null 
                ? DependencyProperty.UnsetValue 
                : Enum.Parse(targetType, parameterString);
        }
    }
}
