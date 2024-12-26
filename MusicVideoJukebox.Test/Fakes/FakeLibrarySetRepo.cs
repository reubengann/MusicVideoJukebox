using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Libraries;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeLibrarySetRepo : ILibrarySetRepo
    {
        public List<LibraryItem> LibraryItems { get; set; } = [];
        public CurrentState CurrentState { get; set; } = new();

        public Task AddLibrary(LibraryItemAdd libraryItem)
        {
            LibraryItems.Add(new LibraryItem { FolderPath = libraryItem.FolderPath, Name = libraryItem.Name, LibraryId = LibraryItems.Count + 1 });
            return Task.CompletedTask;
        }

        public async Task<List<LibraryItem>> GetAllLibraries()
        {
            await Task.CompletedTask;
            return LibraryItems;
        }

        public async Task<List<string>> GetAllLibraryNames()
        {
            await Task.CompletedTask;
            return LibraryItems.Select(x => x.Name).ToList();
        }

        public async Task<List<string>> GetAllLibraryPaths()
        {
            await Task.CompletedTask;
            return LibraryItems.Select(x => x.FolderPath).ToList();
        }

        public async Task<CurrentState> GetCurrentState()
        {
            await Task.CompletedTask;
            return CurrentState;
        }

        public async Task Initialize()
        {
            await Task.CompletedTask;
        }

        public Task UpdateState(CurrentState currentState)
        {
            CurrentState = new CurrentState { LibraryId = currentState.LibraryId, LibraryPath = currentState.LibraryPath, VideoId = currentState.VideoId, Volume = currentState.Volume };
            return Task.CompletedTask;
        }
    }
}