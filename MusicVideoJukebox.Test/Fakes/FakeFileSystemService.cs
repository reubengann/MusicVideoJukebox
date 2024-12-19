using MusicVideoJukebox.Core.Metadata;
using System.IO;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeFileSystemService : IFileSystemService
    {
        public List<string> ExistingFolders = [];
        public List<string> ExistingFiles = [];

        public bool FileExists(string filepath)
        {
            return ExistingFiles.Contains(filepath);
        }

        public bool FolderExists(string path)
        {
            return ExistingFolders.Contains(path);
        }

        public string GetMyDocuments()
        {
            return "mydocs";
        }

        public List<string> ListMp4Files(string folderPath)
        {
            return ExistingFiles.Where(x => x.EndsWith(".mp4")).ToList();
        }
    }
}
