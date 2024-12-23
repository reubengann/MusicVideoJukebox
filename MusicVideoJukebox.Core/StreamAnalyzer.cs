using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
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
        required public double? LUFS { get; set; }
    }

    public class VideoFileAnalyzeFullResult
    {
        required public string Path { get; set; }
        public string? Warning { get; set; }
        required public VideoFileAnalyzeVideoStreamResult VideoStream { get; set; }
        required public VideoFileAnalyzeAudioStreamResult AudioStream { get; set; }
    }

    public class StreamAnalyzer : IStreamAnalyzer
    {
        public async Task<VideoFileAnalyzeFullResult> Analyze(string path)
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

            var loudness = await CalculateLoudness(path);

            return new VideoFileAnalyzeFullResult
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
                    LUFS = loudness
                }
            };
        }

        private async Task<double?> CalculateLoudness(string path)
        {
            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{path}\"")
                .AddParameter("-filter_complex \"loudnorm=I=-23:LRA=7:TP=-2:print_format=json\"")
                .AddParameter("-f null -");

            string ffmpegOutput = string.Empty;
            conversion.OnDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(args.Data))
                {
                    ffmpegOutput += args.Data + Environment.NewLine;
                }
            };

            await conversion.Start();

            // Parse the output for LUFS value
            // Find the line containing '[Parsed_loudnorm_0'
            var loudnormIndex = ffmpegOutput.IndexOf("[Parsed_loudnorm_0");

            if (loudnormIndex >= 0)
            {
                // Extract the portion of the output starting from the loudnorm line
                var loudnormOutput = ffmpegOutput.Substring(loudnormIndex);

                // Find the start and end of the JSON object in the loudnorm section
                var jsonStart = loudnormOutput.IndexOf("{");
                var jsonEnd = loudnormOutput.LastIndexOf("}");

                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var json = loudnormOutput.Substring(jsonStart, jsonEnd - jsonStart + 1);

                    try
                    {
                        // Parse the JSON block
                        var jsonObject = System.Text.Json.JsonDocument.Parse(json);
                        if (jsonObject.RootElement.TryGetProperty("input_i", out var inputIProperty))
                        {
                            if (double.TryParse(inputIProperty.GetString(), out var lufs))
                            {
                                return lufs;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    }
                }
            }
            return null;
        }
    }
}
