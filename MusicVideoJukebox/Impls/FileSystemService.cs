using MusicVideoJukebox.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public List<string> ListMp4Files(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                 .Where(file => string.Equals(Path.GetExtension(file), ".mp4", StringComparison.OrdinalIgnoreCase));

            var relativePaths = files.Select(file => Path.GetRelativePath(folderPath, file)).ToList();

            return relativePaths;
        }
    }
}
