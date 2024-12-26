namespace MusicVideoJukebox.Core.Libraries
{
    public class LibraryItem
    {
        public int LibraryId { get; set; }
        public string FolderPath { get; set; } = null!;

        public string Name { get; set; } = null!;
    }

    public class LibraryItemAdd
    {
        required public string FolderPath { get; set; }
        
        required public string Name { get; set; }
    }

    public interface ILibrarySetRepo
    {
        Task Initialize();
        Task<List<LibraryItem>> GetAllLibraries();
        Task<List<string>> GetAllLibraryPaths();
        Task<List<string>> GetAllLibraryNames();
        Task AddLibrary(LibraryItemAdd libraryItem);
        Task<CurrentState> GetCurrentState();
        Task UpdateState(CurrentState currentState);
    }
}
