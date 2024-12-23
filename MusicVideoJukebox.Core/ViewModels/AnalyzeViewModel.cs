using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class AnalyzeViewModel : AsyncInitializeableViewModel
    {
        private readonly IStreamAnalyzer streamAnalyzer;
        private readonly IUiThreadDispatcher threadDispatcher;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly LibraryStore libraryStore;

        public ObservableCollection<AnalysisResultViewModel> AnalysisResults { get; set; } = [];

        public AnalyzeViewModel(IStreamAnalyzer streamAnalyzer, IUiThreadDispatcher threadDispatcher, IMetadataManagerFactory metadataManagerFactory, LibraryStore libraryStore)
        {
            this.streamAnalyzer = streamAnalyzer;
            this.threadDispatcher = threadDispatcher;
            this.metadataManagerFactory = metadataManagerFactory;
            this.libraryStore = libraryStore;
        }

        public override async Task Initialize()
        {
            _ = Task.Run(LoadThem);
        }

        public async Task LoadThem()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;

            var videoRepo = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
            var videos = await videoRepo.GetAllMetadata();
            var existingAnalysis = (await videoRepo.GetAnalysisResults()).ToDictionary(x => x.VideoId, x => x);

            foreach (var video in videos)
            {
                AnalysisResultViewModel result;

                if (existingAnalysis.ContainsKey(video.VideoId))
                {
                    var existing = existingAnalysis[video.VideoId];
                    ArgumentNullException.ThrowIfNull(existing.Filename);
                    result = new AnalysisResultViewModel
                    {
                        Filename = existing.Filename,
                        VideoCodec = existing.VideoCodec,
                        VideoResolution = existing.VideoResolution,
                        AudioCodec = existing.AudioCodec,
                        LUFS = existing.LUFS,
                        Warning = existing.Warning,
                    };
                }
                else
                {
                    string path = Path.Combine(libraryStore.CurrentState.LibraryPath, video.Filename);
                    try
                    {
                        var analyzeResult = await streamAnalyzer.Analyze(path);

                        await videoRepo.InsertAnalysisResult(new VideoAnalysisEntry
                        {
                            VideoCodec = analyzeResult.VideoStream.Codec,
                            VideoResolution = $"{analyzeResult.VideoStream.Width}x{analyzeResult.VideoStream.Height} @ {analyzeResult.VideoStream.Framerate:F2} FPS",
                            AudioCodec = analyzeResult.AudioStream.Codec,
                            LUFS = analyzeResult.AudioStream.LUFS,
                            Warning = analyzeResult.Warning,
                            VideoId = video.VideoId,
                        });

                        result = new AnalysisResultViewModel
                        {
                            Filename = video.Filename,
                            VideoCodec = analyzeResult.VideoStream.Codec,
                            VideoResolution = $"{analyzeResult.VideoStream.Width}x{analyzeResult.VideoStream.Height} @ {analyzeResult.VideoStream.Framerate:F2} FPS",
                            AudioCodec = analyzeResult.AudioStream.Codec,
                            LUFS = analyzeResult.AudioStream.LUFS,
                            Warning = analyzeResult.Warning,
                        };
                    }
                    catch (Exception ex)
                    {
                        result = new AnalysisResultViewModel
                        {
                            Filename = video.Filename,
                            VideoCodec = "Error",
                            VideoResolution = "N/A",
                            AudioCodec = "Error",
                            Warning = ex.Message,
                            LUFS = null
                        };
                    }
                }
                // Use dispatcher to update UI
                threadDispatcher.Invoke(() =>
                {
                    AnalysisResults.Add(result);
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
