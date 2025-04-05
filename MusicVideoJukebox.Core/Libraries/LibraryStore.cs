using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core.Libraries
{
    public class LibraryStore
    {
        public CurrentState CurrentState { get; set; } = new();
        public int? CurrentPlaylistId { get; set; }


        private readonly ILibrarySetRepo librarySetRepo;
        private readonly IMetadataManagerFactory metadataManagerFactory;

        public LibraryStore(ILibrarySetRepo librarySetRepo, IMetadataManagerFactory metadataManagerFactory)
        {
            this.librarySetRepo = librarySetRepo;
            this.metadataManagerFactory = metadataManagerFactory;
        }

        public async Task Initialize()
        {
            CurrentState = await librarySetRepo.GetCurrentState();
            if (CurrentState.LibraryPath != null)
            {
                // also load the playlist
                var mm = metadataManagerFactory.Create(CurrentState.LibraryPath);
                var activePlaylist = mm.VideoRepo.GetActivePlaylist();
                CurrentPlaylistId = activePlaylist.Id;
            }
        }

        public async Task SetLibrary(int? libraryId, string? folderPath)
        {
            CurrentState.LibraryId = libraryId;
            CurrentState.LibraryPath = folderPath;
            await librarySetRepo.UpdateState(CurrentState);
        }

        public async Task SetVideoId(int videoId)
        {
            CurrentState.VideoId = videoId;
            await librarySetRepo.UpdateState(CurrentState);
        }
    }
}
