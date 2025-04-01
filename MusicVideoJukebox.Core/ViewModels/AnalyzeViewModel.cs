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
        private string operationText = "";

        public DelegateCommand NormalizeSelectedCommand { get; }
        public DelegateCommand CancelOperationsCommand { get; }
        //public DelegateCommand AnalyzeSelectedCommand { get; }
        public DelegateCommand AnalyzeAllCommand { get; }

        public ObservableCollection<AnalysisResultViewModel> AnalysisResults { get; set; } = [];
        public AnalysisResultViewModel? SelectedItem { get; set; }

        public bool IsBusy { get => isBusy; set { SetProperty(ref isBusy, value); RefreshButtons(); } }
        public string OperationText { get => operationText; set => SetProperty(ref operationText, value); }

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
            NormalizeSelectedCommand = new DelegateCommand(NormalizeSelected, () => !IsBusy);
            CancelOperationsCommand = new DelegateCommand(CancelOperations, () => IsBusy);
            //AnalyzeSelectedCommand = new DelegateCommand(AnalyzeSelected, () => !IsBusy);
            AnalyzeAllCommand = new DelegateCommand(AnalyzeAll, () => !IsBusy);
        }

        private void RefreshButtons()
        {
            NormalizeSelectedCommand.RaiseCanExecuteChanged();
            CancelOperationsCommand.RaiseCanExecuteChanged();
            //AnalyzeSelectedCommand.RaiseCanExecuteChanged();
        }

        private void CancelOperations()
        {
            cancellationTokenSource?.Cancel();
        }

        //private async void AnalyzeSelected()
        //{
        //    await AnalyzeSome(AnalysisResults.Where(x => x.IsSelected));
        //}

        private async void AnalyzeAll()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;
            var metadataManager = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
            var changed = await metadataManager.Resync();
            if (changed)
            {
                AnalysisResults.Clear();
                await Initialize();
            }
            await NormalizeSome(AnalysisResults.Where(x => x.LUFS == null));
        }

        private async void NormalizeSelected()
        {
            await NormalizeSome(AnalysisResults.Where(x => x.IsSelected));
        }

        //private async Task AnalyzeSome(IEnumerable<AnalysisResultViewModel> vms)
        //{
        //    if (libraryStore.CurrentState.LibraryPath == null) return;
        //    var videoRepo = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
        //    cancellationTokenSource = new CancellationTokenSource();
        //    if (!vms.Any()) return;
        //    IsBusy = true;

        //    foreach (var video in vms)
        //    {
        //        if (cancellationTokenSource.IsCancellationRequested) return;
        //        var refreshing = video.LUFS != null;
        //        try
        //        {
        //            string path = Path.Combine(libraryStore.CurrentState.LibraryPath, video.Filename);
        //            var analyzeResult = await streamAnalyzer.Analyze(path, cancellationTokenSource.Token);
        //            var warning = analyzeResult.Warning;
        //            if (analyzeResult.AudioStream.LUFS == null)
        //            {
        //                if (!string.IsNullOrEmpty(warning))
        //                    warning += ", ";
        //                warning += "LUFS failed";
        //            }
        //            var videoResolutionStr = $"{analyzeResult.VideoStream.Width}x{analyzeResult.VideoStream.Height} @ {analyzeResult.VideoStream.Framerate:F2} FPS";
        //            video.Warning = warning;
        //            video.VideoCodec = analyzeResult.VideoStream.Codec;
        //            video.VideoResolution = videoResolutionStr;
        //            video.LUFS = analyzeResult.AudioStream.LUFS;
        //            video.AudioCodec = analyzeResult.AudioStream.Codec;
        //            if (refreshing)
        //            {
        //                await videoRepo.UpdateAnalysisResult(video.Entry);
        //            }
        //            else
        //            {
        //                await videoRepo.InsertAnalysisResult(video.Entry);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            video.Warning = ex.Message;
        //        }
        //    }

        //    IsBusy = false;
        //}


        private async Task NormalizeSome(IEnumerable<AnalysisResultViewModel> selectedItems)
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;
            
            cancellationTokenSource = new CancellationTokenSource();
            //var selectedItems = AnalysisResults.Where(x => x.IsSelected).ToList();
            if (!selectedItems.Any()) return;
            IsBusy = true;
            try
            {
                if (cancellationTokenSource.IsCancellationRequested) { IsBusy = false; return; }

                var videoRepo = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
                foreach (var item in selectedItems)
                {
                    var refreshing = item.LUFS != null;
                    OperationText = $"Normalizing {item.Filename}";
                    string path = Path.Combine(libraryStore.CurrentState.LibraryPath, item.Filename);
                    // Analyze it first
                    var analyzeResult = await streamAnalyzer.Analyze(path, cancellationTokenSource.Token);
                    var warning = analyzeResult.Warning;
                    if (analyzeResult.AudioStream.LUFS == null)
                    {
                        if (!string.IsNullOrEmpty(warning))
                            warning += ", ";
                        warning += "LUFS failed";
                    }
                    var videoResolutionStr = $"{analyzeResult.VideoStream.Width}x{analyzeResult.VideoStream.Height} @ {analyzeResult.VideoStream.Framerate:F2} FPS";
                    item.Warning = warning;
                    item.VideoCodec = analyzeResult.VideoStream.Codec;
                    item.VideoWidth = analyzeResult.VideoStream.Width;
                    item.VideoHeight = analyzeResult.VideoStream.Height;
                    item.LUFS = analyzeResult.AudioStream.LUFS;
                    item.AudioCodec = analyzeResult.AudioStream.Codec;
                    await videoRepo.UpdateVideoMetadata(item.Entry);
                    if (item.LUFS == null)
                    {
                        // we cannot continue here
                        continue;
                    }
                    var success = await audioNormalizer.NormalizeAudio(libraryStore.CurrentState.LibraryPath, item.Filename, item.LUFS, CancellationToken.None);
                    if (success != NormalizeAudioResult.DONE) continue;

                    var newResult = await streamAnalyzer.Analyze(path, cancellationTokenSource.Token);
                    // This step can't be canceled. Otherwise, we might leave the database in an incorrect state.
                    // If I wasn't being lazy, I would copy the old file back at this point.
                    await videoRepo.UpdateVideoMetadata(item.Entry);
                    item.LUFS = newResult.AudioStream.LUFS;
                }
            }
            catch(OperationCanceledException)
            {

            }
            finally
            {
                OperationText = "";
                IsBusy = false;
            }
        }

        public override async Task Initialize()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;
            var videoRepo = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
            var videoData = await videoRepo.GetAllMetadata();
            videoData = videoData.OrderBy(x => x.Filename.StartsWith("The ") ? x.Filename[4..] : x.Filename).ToList();

            cancellationTokenSource = new CancellationTokenSource();

            foreach (var video in videoData)
            {
                if (cancellationTokenSource.IsCancellationRequested) return;

                // Create an AnalysisResultViewModel for each record
                var result = new AnalysisResultViewModel(video);
                AnalysisResults.Add(result);
            }
        }
    }

    public class AnalysisResultViewModel : BaseViewModel
    {
        private readonly VideoMetadata entry;
        public VideoMetadata Entry => entry;
        private bool isSelected;

        public AnalysisResultViewModel(VideoMetadata entry)
        {
            this.entry = entry;
        }

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public int VideoId { get => entry.VideoId; set { SetUnderlyingProperty(entry.VideoId, value, v => entry.VideoId = v); } }
        public string Filename { get => entry.Filename; set { SetUnderlyingProperty(entry.Filename, value, v => entry.Filename = v); } }
        public string VideoCodec { get => entry.VideoCodec ?? string.Empty; set { SetUnderlyingProperty(entry.VideoCodec, value, v => entry.VideoCodec = v); } }
        public int? VideoWidth { get => entry.VideoWidth; set { SetUnderlyingProperty(entry.VideoWidth, value, v => entry.VideoWidth = v); } }
        public int? VideoHeight { get => entry.VideoHeight; set { SetUnderlyingProperty(entry.VideoHeight, value, v => entry.VideoHeight = v); } }
        public string AudioCodec { get => entry.AudioCodec ?? string.Empty; set { SetUnderlyingProperty(entry.AudioCodec, value, v => entry.AudioCodec = v); } }
        public double? LUFS { get => entry.LUFS; set { SetUnderlyingProperty(entry.LUFS, value, v => entry.LUFS = v); } }
        public string? Warning { get => entry.Warning; set { SetUnderlyingProperty(entry.Warning, value, v => entry.Warning = v); } }
    }
}
