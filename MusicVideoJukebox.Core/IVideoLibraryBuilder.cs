namespace MusicVideoJukebox.Core
{
    public interface IVideoLibraryBuilder
    {
        Task<VideoLibrary> BuildAsync(string folder);
    }
}
