
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IMetadataManager
    {
        Task<bool> EnsureCreated();
    }
}
