using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MusicVideoJukebox
{
    public class BoolToColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool shown)
            {
                return shown ? 2 : 1;
            }

            return 0; // Default fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
