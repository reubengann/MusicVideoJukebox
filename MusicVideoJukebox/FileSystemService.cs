using MusicVideoJukebox.Core;
using System;
using System.IO;

namespace MusicVideoJukebox
{
    public class FileSystemService : IFileSystemService
    {
        public bool FileExists(string filepath)
        {
            return File.Exists(filepath);
        }

        public bool FolderExists(string path)
        {
            return Directory.Exists(path);
        }

        public string GetMyDocuments()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
