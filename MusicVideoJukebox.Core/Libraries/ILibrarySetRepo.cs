namespace MusicVideoJukebox.Core.Libraries
{
    public class LibraryItem
    {
        public int LibraryId { get; set; }
        public string FolderPath { get; set; } = null!;

        required public string Name { get; set; }
    }

    public interface ILibrarySetRepo
    {
        Task Initialize();
        Task<List<LibraryItem>> GetAllLibraries();
    }
}
