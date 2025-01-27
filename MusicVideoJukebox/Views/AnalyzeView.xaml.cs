using MusicVideoJukebox.Core.ViewModels;
using System;
using System.Collections.ObjectModel;
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
