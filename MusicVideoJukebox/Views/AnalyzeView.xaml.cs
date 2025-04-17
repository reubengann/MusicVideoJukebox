using MusicVideoJukebox.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MusicVideoJukebox.Views
{
    /// <summary>
    /// Interaction logic for StreamVolumeView.xaml
    /// </summary>
    public partial class AnalyzeView : UserControl
    {
        public AnalyzeView()
        {
            InitializeComponent();
        }

        private AnalysisResultViewModel? lastSelectedItem;


        // This works around shift+click selecting large ranges with virtualization turned on.
        private void DataGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not DataGrid dataGrid) return;

            var clickedRow = GetClickedRow(e, dataGrid);
            if (clickedRow == null) return;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && lastSelectedItem != null)
            {
                // Get the range between the last selected and the currently clicked row
                var items = ((AnalyzeViewModel)DataContext).AnalysisResults;
                int lastIndex = items.IndexOf(lastSelectedItem);
                int clickedIndex = items.IndexOf(clickedRow);

                if (lastIndex != -1 && clickedIndex != -1)
                {
                    SelectRange(items, Math.Min(lastIndex, clickedIndex), Math.Max(lastIndex, clickedIndex));
                }
            }
            else
            {
                // Single-click without Shift: De-select all programmatically selected rows
                var items = ((AnalyzeViewModel)DataContext).AnalysisResults;
                foreach (var item in items)
                {
                    item.IsSelected = false;
                }

                // Update the last selected item
                lastSelectedItem = clickedRow;
                lastSelectedItem.IsSelected = true; // Select the clicked row
            }
        }

        private AnalysisResultViewModel? GetClickedRow(MouseButtonEventArgs e, DataGrid dataGrid)
        {
            var hit = VisualTreeHelper.HitTest(dataGrid, e.GetPosition(dataGrid));
            if (hit == null) return null;

            var row = hit.VisualHit.GetParentOfType<DataGridRow>();
            return row?.DataContext as AnalysisResultViewModel;
        }

        private void SelectRange(ObservableCollection<AnalysisResultViewModel> items, int startIndex, int endIndex)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                items[i].IsSelected = true;
            }
        }

        private string _searchText = string.Empty;
        private DateTime _lastKeyPressTime = DateTime.MinValue;

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
            if (dataGrid?.ItemsSource is IEnumerable<AnalysisResultViewModel> tracks)
            {
                var matchingItem = tracks.FirstOrDefault(track =>
                    RemoveThe(track.Filename).StartsWith(_searchText, StringComparison.OrdinalIgnoreCase));

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

        private static string RemoveThe(string s)
        {
            // Remove the "The " prefix from the string
            if (s.StartsWith("The ", StringComparison.OrdinalIgnoreCase))
            {
                return s[4..];
            }
            return s;
        }

    }
    public static class VisualTreeHelperExtensions
    {
        public static T? GetParentOfType<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject? parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }
    }
}
