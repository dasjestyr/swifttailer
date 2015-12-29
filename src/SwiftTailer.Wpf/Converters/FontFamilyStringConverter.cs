using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace SwiftTailer.Wpf.Converters
{
    public class FontFamilyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fontName = value as string;
            if (fontName == null) return value;

            var fontFamily = new FontFamily(fontName);
            return fontFamily;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fontFamily = value as FontFamily;
            if (fontFamily == null) return value;

            var fontName = fontFamily.FamilyNames.FirstOrDefault();
            return fontName;
        }
    }
}
