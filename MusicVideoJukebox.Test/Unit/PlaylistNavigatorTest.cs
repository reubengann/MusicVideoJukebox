using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;

namespace MusicVideoJukebox.Test.Unit
{
    public class PlaylistNavigatorTest
    {
        PlaylistNavigator dut;

        public PlaylistNavigatorTest()
        {
            dut = new PlaylistNavigator([
                new PlaylistTrack { Artist = "artist 1", FileName = "filename1.mp4", Title = "title 1", PlayOrder = 1, VideoId = 1},
                new PlaylistTrack { Artist = "artist 2", FileName = "filename2.mp4", Title = "title 2", PlayOrder = 2, VideoId = 2},
                new PlaylistTrack { Artist = "artist 3", FileName = "filename3.mp4", Title = "title 3", PlayOrder = 3, VideoId = 3},
                new PlaylistTrack { Artist = "artist 4", FileName = "filename4.mp4", Title = "title 4", PlayOrder = 4, VideoId = 4},
                ]);
        }

        [Fact]
        public void DefaultsToFirstTrack()
        {
            Assert.Equal("artist 1", dut.CurrentTrack?.Artist);
        }

        [Fact]
        public void GoesToNextTrack()
        {
            var foo = dut.Next() ?? throw new Exception();
            Assert.Equal("artist 2", foo.Artist);
        }

        [Fact]
        public void WhenGoingToNextTrackAlsoUpdateTheDatabase()
        {
            var foo = dut.Next() ?? throw new Exception();
            Assert.Equal("artist 2", foo.Artist);
        }

        [Fact]
        public void PreviousWrapsAround()
        {
            var foo = dut.Previous() ?? throw new Exception();
            Assert.Equal("artist 4", foo.Artist);
        }

        [Fact]
        public void NextWrapsAround()
        {
            dut.SetCurrentTrack(4);
            var foo = dut.Next();
            Assert.Equal("artist 1", foo.Artist);
        }
    }
}
