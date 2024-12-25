using MusicVideoJukebox.Core.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace MusicVideoJukebox.Views
{
    public partial class PlaylistSelectView : UserControl
    {
        private DispatcherTimer scrollTimer;
        private double scrollDirection; // -1 for left, 1 for right

        public PlaylistSelectView()
        {
            InitializeComponent();

            scrollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            scrollTimer.Tick += ScrollTimer_Tick;
        }

        private void ScrollTimer_Tick(object? sender, EventArgs e)
        {
            if (scrollviewer != null)
            {
                double newOffset = scrollviewer.HorizontalOffset + (scrollDirection * 20); // Adjust speed by changing 10
                scrollviewer.ScrollToHorizontalOffset(Math.Max(0, Math.Min(newOffset, scrollviewer.ScrollableWidth)));
            }
        }

        private void ScrollLeft_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            scrollDirection = -1; // Scroll left
            scrollTimer.Start();
        }

        private void ScrollRight_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            scrollDirection = 1; // Scroll right
            scrollTimer.Start();
        }

        private void Scroll_Stop(object sender, MouseButtonEventArgs e)
        {
            scrollTimer.Stop();
        }

        // Sadly needed since the signature is different.
        private void Scroll_StopOnLeave(object sender, MouseEventArgs e)
        {
            scrollTimer.Stop();
        }
    }
}
