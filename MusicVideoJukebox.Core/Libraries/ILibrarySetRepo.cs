namespace MusicVideoJukebox.Core.Libraries
{
    public class LibraryItem
    {
        public int LibraryId { get; set; }
        public string FolderPath { get; set; } = null!;

        public string Name { get; set; } = null!;
    }

    public interface ILibrarySetRepo
    {
        Task Initialize();
        Task<List<LibraryItem>> GetAllLibraries();
        Task<List<string>> GetAllLibraryPaths();
        Task<List<string>> GetAllLibraryNames();
    }
}
