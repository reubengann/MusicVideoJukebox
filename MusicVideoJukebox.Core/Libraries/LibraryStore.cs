
using MusicVideoJukebox.Core.Metadata;

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

        public async Task SetPlaylist(int? playlistId)
        {
            CurrentState.PlaylistId = playlistId;
            await librarySetRepo.UpdateState(CurrentState);
        }

        public async Task SetVideoId(int videoId)
        {
            CurrentState.VideoId = videoId;
            await librarySetRepo.UpdateState(CurrentState);
        }
    }
}
