using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class AnalyzeViewModelTest
    {
        AnalyzeViewModel dut;
        FakeStreamAnalyzer streamAnalyzer;
        FakeThreadDispatcher threadDispatcher;
        LibraryStore libraryStore;
        FakeLibrarySetRepo librarySetRepo;
        FakeMetadataManagerFactory videoRepo;
        FakeAudioNormalizer audioNormalizer;

        public AnalyzeViewModelTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            libraryStore = new LibraryStore(librarySetRepo);
            libraryStore.SetLibrary(1, "foobar").Wait();
            threadDispatcher = new FakeThreadDispatcher();
            streamAnalyzer = new FakeStreamAnalyzer();
            videoRepo = new FakeMetadataManagerFactory();
            audioNormalizer = new FakeAudioNormalizer();
            dut = new AnalyzeViewModel(streamAnalyzer, threadDispatcher, videoRepo, libraryStore, audioNormalizer);
        }

        [Fact]
        public async Task WhenNotInDbThenAnalyzeThem()
        {
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "track1.mp4", Title = "track 1" });
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 2, Artist = "artist 2", Filename = "track2.mp4", Title = "track 2" });
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 3, Artist = "artist 3", Filename = "track3.mp4", Title = "track 3" });
            await dut.Initialize();
            await Task.Delay(10); // thread dispatcher
            Assert.Equal(3, dut.AnalysisResults.Count);
            Assert.Equal(3, streamAnalyzer.Analyzed.Count);
        }

        [Fact]
        public async Task StoresAnalyzedInTheDb()
        {
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "track1.mp4", Title = "track 1" });
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 2, Artist = "artist 2", Filename = "track2.mp4", Title = "track 2" });
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 3, Artist = "artist 3", Filename = "track3.mp4", Title = "track 3" });
            await dut.Initialize();
            await Task.Delay(10); // thread dispatcher
            Assert.Equal(3, videoRepo.ToReturn.AnalysisEntries.Count);
            Assert.Equal(3, dut.AnalysisResults.Count);
        }

        [Fact]
        public async Task WhenAlreadyDoneInDBDontDoAgain()
        {
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "track1.mp4", Title = "track 1" });
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 2, Artist = "artist 2", Filename = "track2.mp4", Title = "track 2" });
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 3, Artist = "artist 3", Filename = "track3.mp4", Title = "track 3" });
            videoRepo.ToReturn.AnalysisEntries.Add(new VideoAnalysisEntry { VideoId = 1, AudioCodec = "aac", Filename = "track1.mp4", LUFS = -23, VideoCodec = "avi", VideoResolution = "640x480", Warning = null });
            await dut.Initialize();
            await Task.Delay(10); // thread dispatcher
            Assert.Equal(3, dut.AnalysisResults.Count);
            Assert.DoesNotContain(streamAnalyzer.Analyzed, x => x == "foobar\\track1.mp4");
        }

        [Fact]
        public async Task WhenNormalizingDoIt()
        {
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "track1.mp4", Title = "track 1" });
            videoRepo.ToReturn.AnalysisEntries.Add(new VideoAnalysisEntry { VideoId = 1, AudioCodec = "aac", Filename = "track1.mp4", LUFS = -23, VideoCodec = "avi", VideoResolution = "640x480", Warning = null });
            await dut.Initialize();
            await Task.Delay(10); // thread dispatcher
            dut.SelectedItem = dut.AnalysisResults[0];
            dut.NormalizeTrackCommand.Execute();
            Assert.Single(audioNormalizer.Normalized);
        }

        [Fact]
        public async Task WhenNormalizingReanalyzeAndUpdateRepo()
        {
            videoRepo.ToReturn.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "track1.mp4", Title = "track 1" });
            videoRepo.ToReturn.AnalysisEntries.Add(new VideoAnalysisEntry { VideoId = 1, AudioCodec = "aac", Filename = "track1.mp4", LUFS = -23, VideoCodec = "avi", VideoResolution = "640x480", Warning = null });
            await dut.Initialize();
            await Task.Delay(10); // thread dispatcher
            dut.SelectedItem = dut.AnalysisResults[0];
            dut.NormalizeTrackCommand.Execute();
            Assert.Equal(1, videoRepo.ToReturn.AnalysisEntries[0].LUFS ?? 100, 0.01);
        }
    }
}
