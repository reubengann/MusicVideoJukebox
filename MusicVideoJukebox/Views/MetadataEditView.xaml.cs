using MusicVideoJukebox.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicVideoJukebox.Views
{
    /// <summary>
    /// Interaction logic for MetadataEditView.xaml
    /// </summary>
    public partial class MetadataEditView : UserControl
    {
        private string _searchText = string.Empty;
        private DateTime _lastKeyPressTime = DateTime.MinValue;

        public MetadataEditView()
        {
            InitializeComponent();
        }

        private void DataGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            const int searchTimeoutMilliseconds = 1000; // Reset search after 1 second of inactivity
            var currentTime = DateTime.Now;

            // Reset search text if the last key press was too long ago
            if ((currentTime - _lastKeyPressTime).TotalMilliseconds > searchTimeoutMilliseconds)
            {
                _searchText = string.Empty;
            }

            _lastKeyPressTime = currentTime;

            // Append the new character to the search text
            _searchText += e.Text;

            // Find the first matching item
            var dataGrid = sender as DataGrid;
            if (dataGrid?.ItemsSource is IEnumerable<VideoMetadataViewModel> tracks)
            {
                var matchingItem = tracks.FirstOrDefault(track =>
                    track.Artist.StartsWith(_searchText, StringComparison.OrdinalIgnoreCase));

                if (matchingItem != null)
                {
                    // Select and scroll to the matching item
                    dataGrid.SelectedItem = matchingItem;
                    dataGrid.UpdateLayout();
                    dataGrid.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        dataGrid.ScrollIntoView(matchingItem);
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true; // Prevent the default sorting behavior

            if (DataContext is MetadataEditViewModel viewModel)
            {
                var dataGrid = sender as DataGrid;
                if (dataGrid == null) return;
                ListSortDirection newSortDirection;
                if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
                {
                    // If the column is not sorted or currently descending, set to ascending
                    newSortDirection = ListSortDirection.Ascending;
                }
                else
                {
                    // If the column is currently ascending, toggle to descending
                    newSortDirection = ListSortDirection.Descending;
                }

                // Clear SortDirection for all other columns
                foreach (var column in dataGrid.Columns)
                {
                    if (column != e.Column)
                    {
                        column.SortDirection = null;
                    }
                }
                e.Column.SortDirection = newSortDirection;
                viewModel.SortItems(e.Column.SortMemberPath, newSortDirection == ListSortDirection.Ascending);
            }
        }
    }
}
