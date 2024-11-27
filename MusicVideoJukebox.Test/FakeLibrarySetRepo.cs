using MusicVideoJukebox.Core.Libraries;

namespace MusicVideoJukebox.Test
{
    internal class FakeLibrarySetRepo : ILibrarySetRepo
    {
        public List<LibraryItem> LibraryItems { get; set; } = [];

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

        public async Task Initialize()
        {
            await Task.CompletedTask;
        }
    }
}