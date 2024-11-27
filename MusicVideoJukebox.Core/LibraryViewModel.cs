using MusicVideoJukebox.Core.Libraries;
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

        public ObservableCollection<LibraryItemViewModel> Items { get; } = new ObservableCollection<LibraryItemViewModel>();
        public ICommand EditLibraryCommand { get; }
        public ICommand SelectLibraryCommand { get; }

        public LibraryViewModel(ILibrarySetRepo librarySetRepo, IWindowLauncher windowLauncher)
        {
            EditLibraryCommand = new DelegateCommand<LibraryItemViewModel>(EditLibrary);
            SelectLibraryCommand = new DelegateCommand<LibraryItemViewModel>(SelectLibrary);
            this.librarySetRepo = librarySetRepo;
            this.windowLauncher = windowLauncher;
        }

        private void SelectLibrary(LibraryItemViewModel library)
        {
            if (library == null) return;

            if (library.IsAddNew)
            {
                var result = windowLauncher.LaunchAddLibraryDialog();
                if (result.Accepted)
                {

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
