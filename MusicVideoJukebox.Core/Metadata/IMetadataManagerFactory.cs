
namespace MusicVideoJukebox.Core.Metadata
{
    public interface IMetadataManagerFactory
    {
        IMetadataManager Create(string folderPath);
    }
}
