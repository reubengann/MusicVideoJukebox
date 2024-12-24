using MusicVideoJukebox.Core.Audio;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeAudioNormalizer : IAudioNormalizer
    {
        public bool ReturnValue = true;
        public List<string> Normalized = [];

        public Task<bool> NormalizeAudio(string sourcefolder, string filename, double? lufs, CancellationToken cancellationToken)
        {
            Normalized.Add(filename);
            return Task.FromResult(ReturnValue);
        }
    }
}
