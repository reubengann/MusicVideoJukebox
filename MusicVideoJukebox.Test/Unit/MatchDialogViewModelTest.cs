using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class MatchDialogViewModelTest
    {
        MatchDialogViewModel dut;
        FakeMetadataManager metadataManager;

        public MatchDialogViewModelTest()
        {
            metadataManager = new FakeMetadataManager("");
            dut = new MatchDialogViewModel(new VideoMetadata { Artist = "artist1", Filename = "", Title = "track1" }, metadataManager);
        }

        [Fact]
        public void Prepopulates()
        {
            Assert.Equal("artist1%track1", dut.QueryString);
        }

        [Fact]
        public async Task WhenInitializingPopulateTheList()
        {
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album1", Artist = "artist1", ReleaseYear = 1901, Title = "track1" });
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album2", Artist = "artist2", ReleaseYear = 1902, Title = "track2" });
            await dut.Initialize();
            Assert.Equal(2, dut.Candidates.Count);
        }

        [Fact]
        public async Task CanSearchAgain()
        {
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album1", Artist = "artist1", ReleaseYear = 1901, Title = "track1" });
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album2", Artist = "artist2", ReleaseYear = 1902, Title = "track2" });
            await dut.Initialize();
            dut.SearchCommand.Execute(null);
            Assert.Equal(2, metadataManager.SearchCount);
        }
    }
}
