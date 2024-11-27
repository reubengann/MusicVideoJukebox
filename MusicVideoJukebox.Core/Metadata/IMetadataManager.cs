namespace MusicVideoJukebox.Core.Metadata
{
    public interface IMetadataManager
    {
        bool HasMetadata(string folderPath);
        bool CreateMetadata(string folderPath);
    }
}
