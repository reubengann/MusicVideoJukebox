
namespace MusicVideoJukebox.Core.Audio
{
    public interface IAudioNormalizer
    {
        Task<bool> NormalizeAudio(string sourcefolder, string filename, double? lufs, CancellationToken cancellationToken);
    }
}