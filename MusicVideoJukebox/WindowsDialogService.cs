using Microsoft.Win32;
using MusicVideoJukebox.Core;
using System.Windows;

namespace MusicVideoJukebox
{
    public class WindowsDialogService : IDialogService
    {
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

        public void ShutDownApp()
        {
            Application.Current.Shutdown();
        }
    }
}
