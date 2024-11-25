using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core
{
    public class LibraryItem
    {
        public string? Icon { get; set; }
        required public string Name { get; set; }
        public bool IsAddNew { get; set; }
    }

    public class LibraryViewModel : BaseViewModel
    {
        public ObservableCollection<LibraryItem> Items { get; } = new ObservableCollection<LibraryItem>();

        public LibraryViewModel()
        {
            //Items.Add(new LibraryItem { Icon = "Images/library_music.svg", IsAddNew = false, Name = "Library 1" });
            Items.Add(new LibraryItem { Icon = "/Images/library_add.svg", IsAddNew = true, Name = "Add new"});
        }
    }
}
