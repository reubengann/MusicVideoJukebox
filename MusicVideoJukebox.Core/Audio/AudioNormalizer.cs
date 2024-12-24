using Xabe.FFmpeg;

namespace MusicVideoJukebox.Core.Audio
{
    public class AudioNormalizer : IAudioNormalizer
    {
        private readonly string archiveFolder;
        const double targetLufs = -23;
        const double threshold = 2;

        public AudioNormalizer(string archivefolder)
        {
            this.archiveFolder = archivefolder;
        }

        public async Task<bool> NormalizeAudio(string sourcefolder, string filename, double? lufs)
        {
            if (lufs == null) return false;
            double measuredLufs = lufs ?? 0;
            if (Math.Abs(measuredLufs - targetLufs) < threshold) return false;
            var gain = targetLufs - measuredLufs;
            string gainFilter = $"volume={gain:+0.00;-0.00}dB";

            var tempFilename = Guid.NewGuid().ToString() + ".mp4";

            var inputPath = Path.Combine(sourcefolder, filename);
            var outputPath = Path.Combine(sourcefolder, tempFilename);
            var archivePath = Path.Combine(archiveFolder, filename);

            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{inputPath}\"") // Set input
                .AddParameter("-c:v copy") // Copy video stream without re-encoding
                .AddParameter($"-filter:a \"{gainFilter}\"") // Apply audio gain
                .SetOutput(outputPath);

            try
            {
                await conversion.Start();
                if (!Directory.Exists(archiveFolder))
                {
                    Directory.CreateDirectory(archiveFolder);
                }
                File.Move(inputPath, archivePath, overwrite: true);
                File.Move(outputPath, inputPath, overwrite: true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
