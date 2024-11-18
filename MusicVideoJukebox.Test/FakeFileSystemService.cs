using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeFileSystemService : IFileSystemService
    {
        public bool FolderExists(string path)
        {
            throw new NotImplementedException();
        }

        public string GetMyDocuments()
        {
            return "mydocs";
        }
    }
}
