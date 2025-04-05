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
            Assert.Equal("artist1", dut.SearchArtist);
            Assert.Equal("track1", dut.SearchTitle);
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


        [Fact]
        public async Task CannotSelectCommandIfNothingSelected()
        {
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album1", Artist = "artist1", ReleaseYear = 1901, Title = "track1" });
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album2", Artist = "artist2", ReleaseYear = 1902, Title = "track2" });
            await dut.Initialize();
            Assert.False(dut.SelectCommand.CanExecute());
        }

        [Fact]
        public async Task CanSelectCommandIfSomethingSelected()
        {
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album1", Artist = "artist1", ReleaseYear = 1901, Title = "track1" });
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album2", Artist = "artist2", ReleaseYear = 1902, Title = "track2" });
            await dut.Initialize();
            dut.SelectedItem = dut.Candidates.First();
            Assert.True(dut.SelectCommand.CanExecute());
        }

        [Fact]
        public async Task ClosesDialogWhenSelecting()
        {
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album1", Artist = "artist1", ReleaseYear = 1901, Title = "track1" });
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album2", Artist = "artist2", ReleaseYear = 1902, Title = "track2" });
            await dut.Initialize();
            dut.SelectedItem = dut.Candidates.First();
            Assert.True(dut.SelectCommand.CanExecute());
            bool closed = false;
            dut.RequestClose += () => closed = true;
            dut.SelectCommand.Execute();
            Assert.True(closed);
        }

        [Fact]
        public async Task AcceptedWhenSelecting()
        {
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album1", Artist = "artist1", ReleaseYear = 1901, Title = "track1" });
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album2", Artist = "artist2", ReleaseYear = 1902, Title = "track2" });
            await dut.Initialize();
            dut.SelectedItem = dut.Candidates.First();
            Assert.True(dut.SelectCommand.CanExecute());
            dut.SelectCommand.Execute();
            Assert.True(dut.Accepted);
        }

        [Fact]
        public async Task NoAcceptWhenCanceling()
        {
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album1", Artist = "artist1", ReleaseYear = 1901, Title = "track1" });
            metadataManager.ScoredCandidates.Add(new SearchResult { AlbumTitle = "album2", Artist = "artist2", ReleaseYear = 1902, Title = "track2" });
            await dut.Initialize();
            dut.SelectedItem = dut.Candidates.First();
            Assert.True(dut.SelectCommand.CanExecute());
            dut.CancelCommand.Execute(null);
            Assert.False(dut.Accepted);
        }
    }
}
