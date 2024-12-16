using MusicVideoJukebox.Core.ViewModels;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MusicVideoJukebox.Views
{
    public partial class PlayingView : UserControl
    {
        public PlayingView()
        {
            InitializeComponent();
            progressSlider.AddHandler(
        PreviewMouseLeftButtonDownEvent,
        new MouseButtonEventHandler(Slider_PreviewMouseLeftButtonDown_Workaround),
        true);
        }

        private void Slider_PreviewMouseLeftButtonDown_Workaround(object sender, MouseButtonEventArgs e)
        {
            if (sender is Slider slider)
            {
                Track? track = slider.Template.FindName("PART_Track", slider) as Track;
                if (track == null) return;
                
                // Check if the user clicked on the track but not on the thumb
                if (track?.Thumb != null && !track.Thumb.IsMouseOver)
                {
                    if (DataContext is VideoPlayingViewModel vm)
                    {
                        vm.StartScrubbing();
                    }

                    // Update the Slider value to match the clicked position
                    var clickPoint = e.GetPosition(track);
                    double relativeClickPosition = clickPoint.X / track.ActualWidth;
                    slider.Value = slider.Minimum + (relativeClickPosition * (slider.Maximum - slider.Minimum));
                }
            }
        }

        private void Slider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is VideoPlayingViewModel vm)
            {
                vm.StartScrubbing();
            }
        }

        private void Slider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is VideoPlayingViewModel vm)
            {
                vm.StopScrubbing();
            }
        }
    }
}
