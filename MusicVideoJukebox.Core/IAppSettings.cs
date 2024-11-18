
namespace MusicVideoJukebox.Core
{
    public interface IAppSettings
    {
        string? VideoLibraryPath { get; set; }

        Task Save();
        void UpdateVideoLibraryPath(string folderName);
    }
}