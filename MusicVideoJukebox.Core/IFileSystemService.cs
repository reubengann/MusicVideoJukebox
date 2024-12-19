namespace MusicVideoJukebox.Core
{
    public interface IFileSystemService
    {
        bool FileExists(string filepath);
        bool FolderExists(string path);
        List<string> ListMp4Files(string folderPath);
    }
}
