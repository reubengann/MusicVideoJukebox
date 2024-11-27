using MusicVideoJukebox.Core.Libraries;

namespace MusicVideoJukebox.Test
{
    internal class FakeLibrarySetRepo : ILibrarySetRepo
    {
        public Task<List<LibraryItem>> GetAllLibraries()
        {
            throw new NotImplementedException();
        }

        public Task Initialize()
        {
            throw new NotImplementedException();
        }
    }
}