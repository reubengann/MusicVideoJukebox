using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.Navigation;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox.Core
{
    public class LibraryItemViewModel
    {
        public LibraryItem? LibraryItem { get; set; }
        public string? Icon { get; set; }
        public bool IsAddNew { get; set; }
        public string Name => IsAddNew ? "Add new" : LibraryItem?.Name ?? "No library";
    }

    public class LibraryViewModel : AsyncInitializeableViewModel
    {
        private readonly ILibrarySetRepo librarySetRepo;
        private readonly IWindowLauncher windowLauncher;
        private readonly IMetadataManager metadataManager;
        private readonly IDialogService dialogService;

        public ObservableCollection<LibraryItemViewModel> Items { get; } = new ObservableCollection<LibraryItemViewModel>();
        public ICommand EditLibraryCommand { get; }
        public ICommand SelectLibraryCommand { get; }

        public LibraryViewModel(ILibrarySetRepo librarySetRepo, 
            IWindowLauncher windowLauncher, 
            IMetadataManager metadataManager,
            IDialogService dialogService)
        {
            EditLibraryCommand = new DelegateCommand<LibraryItemViewModel>(EditLibrary);
            SelectLibraryCommand = new DelegateCommand<LibraryItemViewModel>(SelectLibrary);
            this.librarySetRepo = librarySetRepo;
            this.windowLauncher = windowLauncher;
            this.metadataManager = metadataManager;
            this.dialogService = dialogService;
        }

        private void SelectLibrary(LibraryItemViewModel library)
        {
            if (library == null) return;

            if (library.IsAddNew)
            {
                var result = windowLauncher.LaunchAddLibraryDialog();
                if (result.Accepted)
                {
                    ArgumentNullException.ThrowIfNull(result.Path);
                    if (!metadataManager.HasMetadata(result.Path))
                    {
                        var success = metadataManager.CreateMetadata(result.Path);
                        if (!success)
                        {
                            dialogService.ShowError("Didn't find metadata in {result.Path}, and could not create it");
                            return;
                        }
                    }
                    // create at least a basic metadata
                    // store in library
                }
            }
            else
            {
                // Logic to select an existing library
            }
        }

        private void EditLibrary(LibraryItemViewModel item)
        {
            // Open the edit window
        }

        override public async Task Initialize()
        {
            Items.Clear();
            var libs = await librarySetRepo.GetAllLibraries();
            foreach (var lib in libs)
            {
                Items.Add(new LibraryItemViewModel { LibraryItem = lib, Icon = "Images/library_music.svg", IsAddNew = false });
            }
            Items.Add(new LibraryItemViewModel { LibraryItem = null, Icon = "/Images/library_add.svg", IsAddNew = true });
        }
    }
}
