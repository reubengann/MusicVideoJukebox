using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test.Unit
{
    internal class FakeStreamAnalyzer : IStreamAnalyzer
    {
        public async Task<VideoFileAnalyzeResult> Analyze(string path)
        {
            return new VideoFileAnalyzeResult
            {
                AudioStream = new VideoFileAnalyzeAudioStreamResult { Bitrate = 1, Channels = 2, Codec = "", SampleRate = 1 },
                Path = path,
                VideoStream = new VideoFileAnalyzeVideoStreamResult { Bitrate = 1, Codec = "", Framerate = 1, Height = 2, Width = 3 },
                Warning = null
            };
        }
    }
}