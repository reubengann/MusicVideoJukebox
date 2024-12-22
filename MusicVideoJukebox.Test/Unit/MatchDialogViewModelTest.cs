using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicVideoJukebox.Test.Unit
{
    public class MatchDialogViewModelTest
    {
        MatchDialogViewModel dut;
        FakeMetadataManager metadataManager;

        public MatchDialogViewModelTest()
        {
            metadataManager = new FakeMetadataManager("");
            dut = new MatchDialogViewModel(new VideoMetadata {  Artist = "artist1", Filename = "", Title = "track1"}, metadataManager);
        }

        [Fact]
        public async Task WhenInitializingPopulateTheList()
        {
            metadataManager.ScoredCandidates.Add(new ScoredMetadata { AlbumTitle = "album1", ArtistName = "artist1", FirstReleaseDateYear = 1901, Similarity = 81, TrackName = "track1" });
            metadataManager.ScoredCandidates.Add(new ScoredMetadata { AlbumTitle = "album2", ArtistName = "artist2", FirstReleaseDateYear = 1902, Similarity = 82, TrackName = "track2" });
            await dut.Initialize();
            Assert.Equal(2, dut.Candidates.Count);
        }

        [Fact]
        public async Task WhenInitializingPopulateTheListHighestFirst()
        {
            metadataManager.ScoredCandidates.Add(new ScoredMetadata { AlbumTitle = "album1", ArtistName = "artist1", FirstReleaseDateYear = 1901, Similarity = 81, TrackName = "track1" });
            metadataManager.ScoredCandidates.Add(new ScoredMetadata { AlbumTitle = "album2", ArtistName = "artist2", FirstReleaseDateYear = 1902, Similarity = 82, TrackName = "track2" });
            await dut.Initialize();
            Assert.Equal(82, dut.Candidates.First().Similarity);
        }
    }
}
