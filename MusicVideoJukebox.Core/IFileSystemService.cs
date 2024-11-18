namespace MusicVideoJukebox.Core
{
    public interface IFileSystemService
    {
        bool FolderExists(string path);
        string GetMyDocuments();
    }
}
