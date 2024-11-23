using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox.Core
{
    public class ImportViewModel : BaseViewModel 
    {
        public ObservableCollection<string> SelectedFiles { get; } = new();
        public int SelectedFilesCount => SelectedFiles.Count;

        private bool _canImport;
        private readonly IDialogService dialogService;

        public bool CanImport
        {
            get => _canImport;
            private set => _canImport = value;
        }

        public ICommand SelectFilesCommand { get; }
        public ICommand ImportCommand { get; }

        public ImportViewModel(IDialogService dialogService)
        {
            SelectFilesCommand = new DelegateCommand(SelectFiles);
            ImportCommand = new DelegateCommand(Import, () => CanImport);
            this.dialogService = dialogService;
        }

        private void SelectFiles()
        {
            var result = dialogService.PickMultipleFiles("MP4 files (*.mp4)|*.mp4");
            if (result.Accepted)
            {
                ArgumentNullException.ThrowIfNull(result.SelectedFiles);
                SelectedFiles.Clear();
                foreach (var file in result.SelectedFiles)
                {
                    SelectedFiles.Add(file);
                }

                OnPropertyChanged(nameof(SelectedFilesCount));
                CanImport = SelectedFiles.Count > 0;
            }
        }

        private async void Import()
        {
            // Perform import logic here
            foreach (var file in SelectedFiles)
            {
                await ImportFileAsync(file);
            }

            // Clear the list after import
            SelectedFiles.Clear();
            OnPropertyChanged(nameof(SelectedFilesCount));
            CanImport = false;
        }

        private async Task ImportFileAsync(string filePath)
        {
            // Copy the file
            // Normalize the audio (optional)
            // Get the metadata
            // Add it to the database
            // Add it to the "all songs" playlist
        }
    }
}
