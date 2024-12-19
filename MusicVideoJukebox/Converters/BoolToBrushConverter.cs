using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MusicVideoJukebox
{
    public class BoolToBrushConverter : IValueConverter
    {
        public Brush? TrueBrush { get; set; }
        public Brush? FalseBrush { get; set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool)
                return null;

            bool boolValue = (bool)value;
            return boolValue ? TrueBrush : FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
