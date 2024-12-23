using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class AnalyzeViewModel : AsyncInitializeableViewModel
    {
        private readonly IStreamAnalyzer streamAnalyzer;
        private readonly IUiThreadDispatcher threadDispatcher;

        public ObservableCollection<AnalysisResultViewModel> AnalysisResults { get; set; } = [];

        public AnalyzeViewModel(IStreamAnalyzer streamAnalyzer, IUiThreadDispatcher threadDispatcher)
        {
            this.streamAnalyzer = streamAnalyzer;
            this.threadDispatcher = threadDispatcher;
        }

        public override async Task Initialize()
        {
            _ = Task.Run(LoadThem);
        }

        public async Task LoadThem()
        {
            // TEMP
            var videoPaths = new List<string>() { @"C:\Users\Reuben\Videos\Test Folder\AC DC - For those about to rock.mp4", @"C:\Users\Reuben\Videos\Test Folder\Annie Lennox - Why.mp4" };
            foreach (var path in videoPaths)
            {
                var result = await streamAnalyzer.Analyze(path);

                // Use dispatcher to update UI
                threadDispatcher.Invoke(() =>
                {
                    AnalysisResults.Add(new AnalysisResultViewModel
                    {
                        Filename = result.Path,
                        VideoCodec = result.VideoStream.Codec,
                        VideoResolution = $"{result.VideoStream.Width}x{result.VideoStream.Height} @ {result.VideoStream.Framerate:F2} FPS",
                        AudioCodec = result.AudioStream.Codec,
                        LUFS = result.AudioStream.LUFS,
                        Warning = result.Warning,
                    });
                });
            }
        }
    }

    public class AnalysisResultViewModel
    {
        required public string Filename { get; set; }
        required public string VideoCodec { get; set; }
        required public string VideoResolution { get; set; }
        required public string AudioCodec { get; set; }
        required public double? LUFS { get; set; }
        required public string? Warning { get; set; }
    }
}
