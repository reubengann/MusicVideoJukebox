
using MusicVideoJukebox.Core.ViewModels;

namespace MusicVideoJukebox.Core.Metadata
{
    public interface IMetadataManager
    {
        Task EnsureCreated();
        Task<List<VideoMetadata>> GetAllMetadata();
        Task UpdateVideoMetadata(VideoMetadata entry);
    }
}
