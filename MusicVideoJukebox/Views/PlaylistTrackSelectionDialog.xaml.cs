using MusicVideoJukebox.Core.ViewModels;
using System;
using System.Windows;

namespace MusicVideoJukebox.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PlaylistTrackSelectionDialog : Window
    {
        PlaylistTrackSelectionViewModel vm;

        public PlaylistTrackSelectionDialog(PlaylistTrackSelectionViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.RequestedClose += Vm_RequestClose;
            this.vm = vm;
            Loaded += Vm_SelectedTrackChanged;
        }

        private void Vm_SelectedTrackChanged(object sender, RoutedEventArgs e)
        {
            if (vm.SelectedTrack != null)
            {
                // Ensure the selected track is visible in the DataGrid
                TrackDataGrid.ScrollIntoView(vm.SelectedTrack);
            }
        }

        private void Vm_RequestClose()
        {
            Close();
        }

        private void TrackDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as PlaylistTrackSelectionViewModel;
            if (viewModel?.SelectedTrack != null)
            {
                // Trigger the OKCommand logic
                if (viewModel.OKCommand.CanExecute(null))
                {
                    viewModel.OKCommand.Execute(null);
                }
            }
        }
    }
}
