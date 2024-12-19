using Material.Icons;
using MusicVideoJukebox.Core.Metadata;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicVideoJukebox
{
    public class StatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not MetadataStatus status)
            {
                return MaterialIconKind.HelpCircleOutline;
            }

            return status switch
            {
                MetadataStatus.Fetching => MaterialIconKind.CloudSync,
                MetadataStatus.Done => MaterialIconKind.Check,
                MetadataStatus.Manual => MaterialIconKind.Pencil,
                MetadataStatus.NotFound => MaterialIconKind.Alert,
                _ => MaterialIconKind.HelpCircleOutline
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
