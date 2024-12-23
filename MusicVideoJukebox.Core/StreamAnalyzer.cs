using Xabe.FFmpeg;

namespace MusicVideoJukebox.Core
{
    public class VideoFileAnalyzeVideoStreamResult
    {
        required public string Codec { get; set; }
        required public int Width { get; set; }
        required public int Height { get; set; }
        required public double Framerate { get; set; }
        required public long Bitrate { get; set; }
    }

    public class VideoFileAnalyzeAudioStreamResult
    {
        required public string Codec { get; set; }
        required public int Channels { get; set; }
        required public int SampleRate { get; set; }
        required public long Bitrate { get; set; }
    }

    public class VideoFileAnalyzeResult
    {
        required public string Path { get; set; }
        public string? Warning { get; set; }
        required public VideoFileAnalyzeVideoStreamResult VideoStream { get; set; }
        required public VideoFileAnalyzeAudioStreamResult AudioStream { get; set; }
    }

    public class StreamAnalyzer : IStreamAnalyzer
    {
        public async Task<VideoFileAnalyzeResult> Analyze(string path)
        {
            var mediaInfo = await FFmpeg.GetMediaInfo(path);
            var warning = "";
            if (mediaInfo.VideoStreams.Count() > 1)
            {
                warning += "Multiple video streams";
            }
            if (mediaInfo.AudioStreams.Count() > 1)
            {
                if (!string.IsNullOrEmpty(warning))
                    warning += ", ";
                warning += "Multiple audio streams";
            }

            var videoStream = mediaInfo.VideoStreams.First();
            var audioStream = mediaInfo.AudioStreams.First();

            return new VideoFileAnalyzeResult
            {
                Path = mediaInfo.Path,
                Warning = string.IsNullOrEmpty(warning) ? null : warning,
                VideoStream = new VideoFileAnalyzeVideoStreamResult
                {
                    Codec = videoStream.Codec,
                    Width = videoStream.Width,
                    Height = videoStream.Height,
                    Framerate = videoStream.Framerate,
                    Bitrate = videoStream.Bitrate,
                },
                AudioStream = new VideoFileAnalyzeAudioStreamResult
                {
                    Codec = audioStream.Codec,
                    Channels = audioStream.Channels,
                    SampleRate = audioStream.SampleRate,
                    Bitrate = audioStream.Bitrate,
                }
            };
        }
    }
}
