using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class VideoPlayingViewModelTest
    {
        VideoPlayingViewModel dut;
        FakeMediaPlayer2 mediaPlayer2;
        FakeUIThreadFactory threadFactory;
        FakeUiThreadTimer progressBarTimer;
        FakeMetadataManagerFactory metadataManagerFactory;
        LibraryStore libraryStore;

        public VideoPlayingViewModelTest()
        {
            libraryStore = new LibraryStore();
            metadataManagerFactory = new FakeMetadataManagerFactory();  
            threadFactory = new FakeUIThreadFactory();
            progressBarTimer = new FakeUiThreadTimer();
            threadFactory.ToReturn.Add(progressBarTimer);
            mediaPlayer2 = new FakeMediaPlayer2();
            dut = new VideoPlayingViewModel(mediaPlayer2, threadFactory, metadataManagerFactory, libraryStore);
        }

        [Fact]
        public void WhenPausedNotPlaying()
        {
            mediaPlayer2.IsPlaying = true;
            dut.PauseCommand.Execute(null);
            Assert.False(dut.IsPlaying);
            Assert.False(mediaPlayer2.IsPlaying);
        }

        [Fact]
        public void WhenPlayedIsPlaying()
        {
            dut.PlayCommand.Execute(null);
            Assert.True(dut.IsPlaying);
            Assert.True(mediaPlayer2.IsPlaying);
            Assert.True(progressBarTimer.Started);
        }

        [Fact]
        public void EverySoOftenCheckTheMediaPlayer()
        {
            mediaPlayer2.InternalLength = 25;
            mediaPlayer2.InternalPosition = 5;
            progressBarTimer.Trigger();
            Assert.Equal(25, dut.VideoLengthSeconds);
            Assert.Equal(5, dut.VideoPositionTime);
        }

        [Fact]
        public void ChecksTheLibraryStoreWhenTriggered()
        {
            Assert.False(dut.IsPlaying);
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new Core.Metadata.VideoMetadata { Artist = "", Filename = "fake.mp4", Title = "" });
            libraryStore.SetLibrary(1, "something");
            
        }
    }
}
