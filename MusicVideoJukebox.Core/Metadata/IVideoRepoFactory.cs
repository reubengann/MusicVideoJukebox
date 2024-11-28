
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IVideoRepoFactory
    {
        IVideoRepo Create(string folderPath);
    }
}
