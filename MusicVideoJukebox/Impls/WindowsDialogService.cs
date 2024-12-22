using Microsoft.Win32;
using MusicVideoJukebox.Core.UserInterface;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Views;
using System.Windows;

namespace MusicVideoJukebox
{
    public class WindowsDialogService : IDialogService
    {
        private readonly Window parent;

        public WindowsDialogService(Window parent)
        {
            this.parent = parent;
        }

        public MultipleFilePickerResult PickMultipleFiles(string filter)
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter,
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                return new MultipleFilePickerResult { Accepted = true, SelectedFiles = [.. dialog.FileNames] };
            }
            return new MultipleFilePickerResult { Accepted = false };
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public FolderPickerResult ShowFolderSelect(string title, string InitialDirectory)
        {
            var dialog = new OpenFolderDialog
            {
                Title = title,
                InitialDirectory = InitialDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                return new FolderPickerResult { Accepted = true, SelectedFolder = dialog.FolderName };
            }
            return new FolderPickerResult { Accepted = false };
        }

        public MetadataMatchDialogResult ShowMatchDialog(MatchDialogViewModel vm)
        {
            vm.WindowHeight = (int)(parent.ActualHeight * 0.8);
            vm.WindowWidth = (int)(parent.ActualWidth * 0.8);
            var dialog = new MetadataMatchSelectionDialog(vm);
            dialog.Owner = parent;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            var result = dialog.ShowDialog();
            if (vm.Accepted)
            {
                return new MetadataMatchDialogResult { Accepted = true, ScoredMetadata = vm.SelectedItem };
            }
            else
            {
                return new MetadataMatchDialogResult { Accepted = false };
            }
        }

        public void ShutDownApp()
        {
            Application.Current.Shutdown();
        }
    }
}
