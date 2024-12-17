using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MusicVideoJukebox
{
    public class BoolToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string marginParams)
            {
                var parts = marginParams.Split('|'); // Separate the two Thickness strings
                if (parts.Length == 2)
                {
                    var isVisible = value is bool b && b;

                    // Parse the two Thickness values
                    var thicknessTrue = (Thickness)new ThicknessConverter().ConvertFromString(parts[0]);
                    var thicknessFalse = (Thickness)new ThicknessConverter().ConvertFromString(parts[1]);

                    return isVisible ? thicknessTrue : thicknessFalse;
                }
            }

            return new Thickness(0); // Default fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
