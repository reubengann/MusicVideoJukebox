using MusicVideoJukebox.Core.Libraries;
using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core
{
    public class AddLibraryDialogViewModel : BaseViewModel
    {
        private IDialogService dialogService;
        private readonly ILibrarySetRepo librarySetRepo;
        private string folderPath = "";

        public ICommand BrowseFolderCommand { get; }
        public ICommand SaveCommand;
        public ICommand CancelCommand;

        public AddLibraryDialogViewModel(IDialogService dialogService, ILibrarySetRepo librarySetRepo)
        {
            this.dialogService = dialogService;
            this.librarySetRepo = librarySetRepo;
            BrowseFolderCommand = new DelegateCommand(OpenFolderPicker);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void OpenFolderPicker()
        {
            var result = dialogService.ShowFolderSelect("Choose a folder", Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            if (result.Accepted)
            {
                ArgumentNullException.ThrowIfNull(result.SelectedFolder);
                FolderPath = result.SelectedFolder;
            }
        }

        public string LibraryName { get; set; } = "";
        public string FolderPath { get => folderPath; set { folderPath = value; OnPropertyChanged(nameof(FolderPath)); } }

        public event Action<bool>? RequestClose;

        

        private async void Save()
        {
            // Verify this is good data
            if (!Directory.Exists(FolderPath)) 
            {
                dialogService.ShowError($"Folder {FolderPath} does not exist.");
                return;
            }
            var existingFolders = await librarySetRepo.GetAllLibraryPaths();
            if (existingFolders.Contains(FolderPath))
            {
                dialogService.ShowError($"Folder {FolderPath} is already a library.");
                return;
            }
        }
        

        private void Cancel()
        {
        }
    }
}
