using MusicVideoJukebox.Core.Libraries;
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

        public ObservableCollection<LibraryItemViewModel> Items { get; } = new ObservableCollection<LibraryItemViewModel>();
        public ICommand EditLibraryCommand { get; }
        public ICommand SelectLibraryCommand { get; }

        public LibraryViewModel(ILibrarySetRepo librarySetRepo)
        {
            EditLibraryCommand = new DelegateCommand<LibraryItemViewModel>(EditLibrary);
            SelectLibraryCommand = new DelegateCommand<LibraryItemViewModel>(SelectLibrary);
            this.librarySetRepo = librarySetRepo;
        }

        private void SelectLibrary(LibraryItemViewModel library)
        {
            if (library == null) return;

            if (library.IsAddNew)
            {
                // Logic to add a new library
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
            //Items.Add(new LibraryItem { Icon = "Images/library_music.svg", IsAddNew = false, Name = "Library 1" });
            // Get the existing libraries
            Items.Add(new LibraryItemViewModel { LibraryItem = null, Icon = "/Images/library_add.svg", IsAddNew = true });
        }
    }
}
