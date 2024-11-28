namespace MusicVideoJukebox.Core
{
    public interface IFileSystemService
    {
        bool FileExists(string filepath);
        bool FolderExists(string path);
        string GetMyDocuments();
    }
}
