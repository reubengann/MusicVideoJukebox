using SharpVectors.Converters;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MusicVideoJukebox
{
    public class MultiFormatImage : ContentControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(string), typeof(MultiFormatImage),
                new PropertyMetadata(null, OnSourceChanged));

        public string? Source
        {
            get => (string?)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (MultiFormatImage)d;
            control.UpdateContent();
        }

        private void UpdateContent()
        {
            if (string.IsNullOrWhiteSpace(Source))
            {
                Content = null;
                return;
            }

            var extension = Path.GetExtension(Source)?.ToLower();
            if (extension == ".svg")
            {
                var svgViewbox = new SvgViewbox
                {
                    Source = new Uri(Source, UriKind.RelativeOrAbsolute)
                };
                Content = svgViewbox;
            }
            else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
            {
                var image = new Image
                {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Source, UriKind.RelativeOrAbsolute))
                };
                Content = image;
            }
            else
            {
                throw new NotSupportedException($"Unsupported image format: {extension}");
            }
        }
    }
}
