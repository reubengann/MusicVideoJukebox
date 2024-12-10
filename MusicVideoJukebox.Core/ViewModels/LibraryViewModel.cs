using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.Navigation;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class LibraryItemViewModel
    {
        public LibraryItem? LibraryItem { get; set; }
        public string? Icon { get; set; }
        public bool IsAddNew { get; set; }
        public string Name => IsAddNew ? "Add new" : LibraryItem?.Name ?? "No library";

        required public int? LibraryId { get; set; }
    }

    public class LibraryViewModel : AsyncInitializeableViewModel
    {
        private readonly LibraryStore libraryStore;
        private readonly ILibrarySetRepo librarySetRepo;
        private readonly IWindowLauncher windowLauncher;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;

        public ObservableCollection<LibraryItemViewModel> Items { get; } = new ObservableCollection<LibraryItemViewModel>();
        public ICommand EditLibraryCommand { get; }
        public ICommand SelectLibraryCommand { get; }

        public LibraryViewModel(LibraryStore libraryStore,
            ILibrarySetRepo librarySetRepo,
            IWindowLauncher windowLauncher,
            IMetadataManagerFactory metadataManagerFactory,
            IDialogService dialogService,
            INavigationService navigationService
            )
        {
            EditLibraryCommand = new DelegateCommand<LibraryItemViewModel>(EditLibrary);
            SelectLibraryCommand = new DelegateCommand<LibraryItemViewModel>(SelectLibrary);
            this.libraryStore = libraryStore;
            this.librarySetRepo = librarySetRepo;
            this.windowLauncher = windowLauncher;
            this.metadataManagerFactory = metadataManagerFactory;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
        }

        private async void SelectLibrary(LibraryItemViewModel libraryvm)
        {
            if (libraryvm == null) return;

            if (libraryvm.IsAddNew)
            {
                var result = windowLauncher.LaunchAddLibraryDialog();
                if (result.Accepted)
                {
                    ArgumentNullException.ThrowIfNull(result.Path);
                    ArgumentNullException.ThrowIfNull(result.Name);
                    var metadataManager = metadataManagerFactory.Create(result.Path);
                    await metadataManager.EnsureCreated();
                    await librarySetRepo.AddLibrary(new LibraryItemAdd { FolderPath = result.Path, Name = result.Name});
                    await ReloadLibraries();
                }
            }
            else
            {
                ArgumentNullException.ThrowIfNull(libraryvm.LibraryItem);
                libraryStore.SetLibrary(libraryvm.LibraryItem.LibraryId, libraryvm.LibraryItem.FolderPath);
                navigationService.NavigateToNothing();
            }
        }

        private void EditLibrary(LibraryItemViewModel item)
        {
            // Open the edit window
        }

        override public async Task Initialize()
        {
            await ReloadLibraries();
        }

        private async Task ReloadLibraries()
        {
            Items.Clear();
            var libs = await librarySetRepo.GetAllLibraries();
            foreach (var lib in libs)
            {
                var metadataManager = metadataManagerFactory.Create(lib.FolderPath);
                await metadataManager.EnsureCreated();
                Items.Add(new LibraryItemViewModel { LibraryId = lib.LibraryId, LibraryItem = lib, Icon = "Images/library_music.svg", IsAddNew = false });
            }
            Items.Add(new LibraryItemViewModel { LibraryId = null, LibraryItem = null, Icon = "/Images/library_add.svg", IsAddNew = true });
        }
    }
}
