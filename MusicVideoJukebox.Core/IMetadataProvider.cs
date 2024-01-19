namespace MusicVideoJukebox.Core
{
    public interface IMetadataProvider
    {
        Task<VideoInfo> GetMetadata(string artist, string track);
    }
}
