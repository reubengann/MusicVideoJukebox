using MusicVideoJukebox.Core.Audio;
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class AnalyzeViewModel : AsyncInitializeableViewModel
    {
        private readonly IStreamAnalyzer streamAnalyzer;
        private readonly IUiThreadDispatcher threadDispatcher;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly LibraryStore libraryStore;
        private readonly IAudioNormalizer audioNormalizer;
        private CancellationTokenSource? cancellationTokenSource;
        private bool isBusy = false;

        public DelegateCommand NormalizeTrackCommand { get; }
        public DelegateCommand CancelOperationsCommand { get; }
        public DelegateCommand NormalizeAllCommand { get; }
        public DelegateCommand ReanalyzeAllCommand { get; }

        public ObservableCollection<AnalysisResultViewModel> AnalysisResults { get; set; } = [];
        public AnalysisResultViewModel? SelectedItem { get; set; }

        public bool IsBusy { get => isBusy; set { SetProperty(ref isBusy, value); RefreshButtons(); } }

        public AnalyzeViewModel(IStreamAnalyzer streamAnalyzer,
            IUiThreadDispatcher threadDispatcher,
            IMetadataManagerFactory metadataManagerFactory,
            LibraryStore libraryStore,
            IAudioNormalizer audioNormalizer)
        {
            this.streamAnalyzer = streamAnalyzer;
            this.threadDispatcher = threadDispatcher;
            this.metadataManagerFactory = metadataManagerFactory;
            this.libraryStore = libraryStore;
            this.audioNormalizer = audioNormalizer;
            NormalizeTrackCommand = new DelegateCommand(NormalizeSelected, () => !IsBusy);
            CancelOperationsCommand = new DelegateCommand(CancelOperations, () => IsBusy);
            NormalizeAllCommand = new DelegateCommand(NormalizeAll, () => !IsBusy);
            ReanalyzeAllCommand = new DelegateCommand(ReanalyzeAll, () => !IsBusy);
        }

        private void RefreshButtons()
        {
            NormalizeTrackCommand.RaiseCanExecuteChanged();
            CancelOperationsCommand.RaiseCanExecuteChanged();
            NormalizeAllCommand.RaiseCanExecuteChanged();
            ReanalyzeAllCommand.RaiseCanExecuteChanged();
        }

        private void ReanalyzeAll()
        {

        }

        private void NormalizeAll()
        {

        }

        private void CancelOperations()
        {
            cancellationTokenSource?.Cancel();
        }

        private async void NormalizeSelected()
        {
            cancellationTokenSource = new CancellationTokenSource();
            if (SelectedItem == null) return;
            if (libraryStore.CurrentState.LibraryPath == null) return;
            IsBusy = true;
            try
            {
                var success = await audioNormalizer.NormalizeAudio(libraryStore.CurrentState.LibraryPath, SelectedItem.Filename, SelectedItem.LUFS, CancellationToken.None);
                if (!success) return;
            }
            finally
            {
                IsBusy = false;
            }
            if (cancellationTokenSource.IsCancellationRequested) return;
            string path = Path.Combine(libraryStore.CurrentState.LibraryPath, SelectedItem.Filename);
            var newResult = await streamAnalyzer.Analyze(path, cancellationTokenSource.Token);
            var videoRepo = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);

            // This step can't be canceled. Otherwise, we might leave the database in an incorrect state.
            // If I wasn't being lazy, I would copy the old file back at this point.
            await videoRepo.UpdateAnalysisVolume(SelectedItem.VideoId, newResult.AudioStream.LUFS);
            SelectedItem.LUFS = newResult.AudioStream.LUFS;
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

            cancellationTokenSource = new CancellationTokenSource();

            foreach (var video in videos)
            {
                if (cancellationTokenSource.IsCancellationRequested) return;
                AnalysisResultViewModel result;

                if (existingAnalysis.ContainsKey(video.VideoId))
                {
                    var existing = existingAnalysis[video.VideoId];
                    ArgumentNullException.ThrowIfNull(existing.Filename);
                    result = new AnalysisResultViewModel
                    {
                        VideoId = video.VideoId,
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
                        var analyzeResult = await streamAnalyzer.Analyze(path, cancellationTokenSource.Token);

                        var warning = analyzeResult.Warning;
                        if (analyzeResult.AudioStream.LUFS == null)
                        {
                            if (!string.IsNullOrEmpty(warning))
                                warning += ", ";
                            warning += "LUFS failed";
                        }

                        await videoRepo.InsertAnalysisResult(new VideoAnalysisEntry
                        {
                            VideoCodec = analyzeResult.VideoStream.Codec,
                            VideoResolution = $"{analyzeResult.VideoStream.Width}x{analyzeResult.VideoStream.Height} @ {analyzeResult.VideoStream.Framerate:F2} FPS",
                            AudioCodec = analyzeResult.AudioStream.Codec,
                            LUFS = analyzeResult.AudioStream.LUFS,
                            Warning = warning,
                            VideoId = video.VideoId,
                        });

                        result = new AnalysisResultViewModel
                        {
                            VideoId = video.VideoId,
                            Filename = video.Filename,
                            VideoCodec = analyzeResult.VideoStream.Codec,
                            VideoResolution = $"{analyzeResult.VideoStream.Width}x{analyzeResult.VideoStream.Height} @ {analyzeResult.VideoStream.Framerate:F2} FPS",
                            AudioCodec = analyzeResult.AudioStream.Codec,
                            LUFS = analyzeResult.AudioStream.LUFS,
                            Warning = warning,
                        };
                    }
                    catch (Exception ex)
                    {
                        result = new AnalysisResultViewModel
                        {
                            VideoId = video.VideoId,
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

    public class AnalysisResultViewModel : BaseViewModel
    {
        private double? lufs;

        required public int VideoId { get; set; }
        required public string Filename { get; set; }
        required public string VideoCodec { get; set; }
        required public string VideoResolution { get; set; }
        required public string AudioCodec { get; set; }
        required public double? LUFS { get => lufs; set => SetProperty(ref lufs, value); }
        required public string? Warning { get; set; }
    }
}
