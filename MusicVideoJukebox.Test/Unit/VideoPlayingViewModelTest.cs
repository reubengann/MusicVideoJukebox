using MusicVideoJukebox.Core.ViewModels;

namespace MusicVideoJukebox.Test.Unit
{
    public class VideoPlayingViewModelTest
    {
        VideoPlayingViewModel dut;
        FakeMediaPlayer2 mediaPlayer2;

        public VideoPlayingViewModelTest()
        {
            mediaPlayer2 = new FakeMediaPlayer2();
            dut = new VideoPlayingViewModel(mediaPlayer2);
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
        }
    }
}
