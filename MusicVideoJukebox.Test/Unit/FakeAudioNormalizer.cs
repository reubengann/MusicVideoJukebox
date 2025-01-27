using MusicVideoJukebox.Core.Audio;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeAudioNormalizer : IAudioNormalizer
    {
        public NormalizeAudioResult ReturnValue = NormalizeAudioResult.FAILED;
        public List<string> Normalized = [];

        public Task<NormalizeAudioResult> NormalizeAudio(string sourcefolder, string filename, double? lufs, CancellationToken cancellationToken)
        {
            Normalized.Add(filename);
            return Task.FromResult(ReturnValue);
        }
    }
}
