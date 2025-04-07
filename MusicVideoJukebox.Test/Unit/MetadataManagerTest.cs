using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class MetadataManagerTest
    {
        MetadataManager dut;
        FakeVideoRepo videoRepo;
        FakeFileSystemService fileSystemService;
        FakeReferenceDataRepo referenceDataRepo;

        public MetadataManagerTest()
        {
            referenceDataRepo = new FakeReferenceDataRepo();
            fileSystemService = new FakeFileSystemService();
            videoRepo = new FakeVideoRepo();
            dut = new MetadataManager("thepath", videoRepo, fileSystemService, referenceDataRepo);
        }

        [Fact]
        public async Task PopulatesBasicInfo()
        {
            fileSystemService.ExistingFiles.AddRange(["artist 1 - track1.mp4", "artist 2 - track 2.mp4"]);
            await dut.EnsureCreated();
            Assert.True(videoRepo.TablesCreated);
            Assert.Equal(2, videoRepo.MetadataEntries.Count);
            Assert.Equal(["artist 1", "artist 2"], videoRepo.MetadataEntries.Select(x => x.Artist).ToHashSet());
        }

        [Fact]
        public async Task CanGetPlaylistTracksForViewmodel()
        {
            videoRepo.PlaylistTracks.Add(new PlaylistTrack { VideoId = 1, PlaylistId = 1, Artist = "", FileName = "", Title =""});
            videoRepo.PlaylistTracks.Add(new PlaylistTrack { VideoId = 1, PlaylistId = 1, Artist = "", FileName = "", Title =""});
            var result = await dut.GetPlaylistTracksForViewmodel(1);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task WhenShufflingAlsoResetPlayOrder()
        {
            videoRepo.CurrentActivePlaylistStatus.SongOrder = 5;
            await dut.ShuffleTracks(1);
            Assert.Equal(1, videoRepo.CurrentActivePlaylistStatus.SongOrder);
        }

        [Fact]
        public async Task DoesNotRunTableCreateIfAlreadyInitialized()
        {
            videoRepo.TablesCreated = true;
            await dut.EnsureCreated();
            Assert.False(videoRepo.RanTableCreate);
        }

        [Fact]
        public async Task NoChangesWhenFileListingMatches()
        {
            fileSystemService.ExistingFiles.AddRange(["artist 1 - track 1.mp4", "artist 2 - track 2.mp4"]);
            videoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist 1", Filename = "artist 1 - track 1.mp4", Title = "track 1" });
            videoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist 2", Filename = "artist 2 - track 2.mp4", Title = "track 2" });
            var anyChanges = await dut.Resync();
            Assert.False(anyChanges);
        }

        [Fact]
        public async Task AddsNewFiles()
        {
            fileSystemService.ExistingFiles.AddRange(["artist 1 - track 1.mp4", "artist 2 - track 2.mp4"]);
            videoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist 1", Filename = "artist 1 - track 1.mp4", Title = "track 1" });
            var anyChanges = await dut.Resync();
            Assert.True(anyChanges);
            Assert.Equal(2, videoRepo.MetadataEntries.Count);
            Assert.Equal("track 2", videoRepo.MetadataEntries[1].Title);
            Assert.Equal("artist 2", videoRepo.MetadataEntries[1].Artist);
        }

        [Fact]
        public async Task RemovesDeletedFiles()
        {
            fileSystemService.ExistingFiles.AddRange(["artist 1 - track 1.mp4"]);
            videoRepo.MetadataEntries.Add(new VideoMetadata { VideoId = 1, Artist = "artist 1", Filename = "artist 1 - track 1.mp4", Title = "track 1" });
            videoRepo.MetadataEntries.Add(new VideoMetadata { VideoId = 2, Artist = "artist 2", Filename = "artist 2 - track 2.mp4", Title = "track 2" });
            var anyChanges = await dut.Resync();
            Assert.True(anyChanges);
            Assert.Single(videoRepo.MetadataEntries);
        }
    }
}
