using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicVideoJukebox
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isBright && isBright)
            {
                return 1.0; // Bright
            }
            else
            {
                return 0.5; // Dim
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
