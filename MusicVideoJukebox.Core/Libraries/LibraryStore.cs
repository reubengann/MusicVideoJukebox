namespace MusicVideoJukebox.Core.Libraries
{
    public class LibraryStore
    {
        public CurrentState CurrentState { get; set; } = new();

        private readonly ILibrarySetRepo librarySetRepo;

        public LibraryStore(ILibrarySetRepo librarySetRepo)
        {
            this.librarySetRepo = librarySetRepo;
        }

        public async Task Initialize()
        {
            CurrentState = await librarySetRepo.GetCurrentState();
        }

        public async Task SetLibrary(int? libraryId, string? folderPath)
        {
            CurrentState.LibraryId = libraryId;
            CurrentState.LibraryPath = folderPath;
            await librarySetRepo.UpdateState(CurrentState);
        }

        public void SetPlaylist(int? playlistId)
        {
            CurrentState.PlaylistId = playlistId;
        }
    }
}
