using MusicVideoJukebox.Core.Metadata;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MusicVideoJukebox
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not MetadataStatus status) return Brushes.Gray;

            return status switch
            {
                MetadataStatus.Fetching => Brushes.Yellow,
                MetadataStatus.Done => Brushes.Green,
                MetadataStatus.Manual => Brushes.Orange,
                MetadataStatus.NotFound => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
