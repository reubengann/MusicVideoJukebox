
namespace MusicVideoJukebox.Core.Metadata
{
    public class VideoRepoFactory : IVideoRepoFactory
    {
        public IVideoRepo Create(string folderPath)
        {
            return new VideoRepo(folderPath);
        }
    }
}
