using MusicVideoJukebox.Core.Audio;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeStreamAnalyzer : IStreamAnalyzer
    {
        public List<string> Analyzed = [];

        public async Task<VideoFileAnalyzeFullResult> Analyze(string path)
        {
            Analyzed.Add(path);
            await Task.CompletedTask;
            return new VideoFileAnalyzeFullResult
            {
                AudioStream = new VideoFileAnalyzeAudioStreamResult { Bitrate = 1, Channels = 2, Codec = "", SampleRate = 1, LUFS = 1 },
                Path = path,
                VideoStream = new VideoFileAnalyzeVideoStreamResult { Bitrate = 1, Codec = "", Framerate = 1, Height = 2, Width = 3 },
                Warning = null
            };
        }
    }
}