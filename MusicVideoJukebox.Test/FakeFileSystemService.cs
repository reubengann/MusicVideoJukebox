using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    internal class FakeFileSystemService : IFileSystemService
    {
        public List<string> ExistingPaths = [];

        public bool FolderExists(string path)
        {
            return ExistingPaths.Contains(path);
        }

        public string GetMyDocuments()
        {
            return "mydocs";
        }
    }
}
