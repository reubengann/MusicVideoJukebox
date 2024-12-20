using MusicVideoJukebox.Core.Libraries;

namespace MusicVideoJukebox.Core
{
    public class AppStateService
    {
        private readonly ILibrarySetRepo librarySetRepo;

        public AppStateService(ILibrarySetRepo librarySetRepo)
        {
            this.librarySetRepo = librarySetRepo;
        }

        public CurrentState CurrentState { get; set; } = null!;

        public async Task Initialize()
        {
            CurrentState = await librarySetRepo.GetCurrentState();
        }

        public async Task UpdateLibraryId(int libraryId)
        {
            CurrentState.LibraryId = libraryId;
            await librarySetRepo.UpdateState(CurrentState);
        }
    }
}
