using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicVideoJukebox
{
    public class FractionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double size && double.TryParse(parameter?.ToString(), out double fraction))
            {
                return size * fraction;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
