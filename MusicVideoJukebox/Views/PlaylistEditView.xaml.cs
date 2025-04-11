using MusicVideoJukebox.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicVideoJukebox.Views
{
    /// <summary>
    /// Interaction logic for PlaylistsView.xaml
    /// </summary>
    public partial class PlaylistEditView : UserControl
    {
        public PlaylistEditView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                // Deselect items that were removed
                foreach (AvailableTrackViewModel item in e.RemovedItems)
                {
                    item.IsSelected = false;
                }

                // Select items that were added
                foreach (AvailableTrackViewModel item in e.AddedItems)
                {
                    item.IsSelected = true;
                }
            }
        }

        private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                var scrollViewer = FindScrollViewer(listBox);
                if (scrollViewer != null)
                {
                    double offsetChange = e.Delta > 0 ? -1 : 1; // Adjust the scroll amount here
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offsetChange);
                    e.Handled = true;
                }
            }
        }

        private static ScrollViewer? FindScrollViewer(DependencyObject obj)
        {
            if (obj is ScrollViewer viewer)
                return viewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var result = FindScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
