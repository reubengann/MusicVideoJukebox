using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core
{
    public class LibraryItem
    {
        public string? Icon { get; set; }
        required public string Name { get; set; }
        public bool IsAddButton { get; set; } // Flag to differentiate "Add Library"
    }

    public class LibraryViewModel : BaseViewModel
    {
        public ObservableCollection<LibraryItem> Items { get; } = new ObservableCollection<LibraryItem>();

        public LibraryViewModel()
        {
            Items.Add(new LibraryItem { Icon = "Images/library_music.svg", IsAddButton = false, Name = "Library 1" });
            Items.Add(new LibraryItem { Icon = "/Images/library_add.svg", IsAddButton = true, Name = "Add new"});
        }
    }
}