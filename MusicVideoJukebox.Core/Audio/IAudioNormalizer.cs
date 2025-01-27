
namespace MusicVideoJukebox.Core.Audio
{
    public enum NormalizeAudioResult
    {
        FAILED = 0,
        NOT_NEEDED = 1,
        DONE = 2
    }

    public interface IAudioNormalizer
    {
        Task<NormalizeAudioResult> NormalizeAudio(string sourcefolder, string filename, double? lufs, CancellationToken cancellationToken);
    }
}