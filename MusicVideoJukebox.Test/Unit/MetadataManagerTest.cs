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
            Assert.Equal(new[] { "artist 1", "artist 2" }.ToHashSet(), videoRepo.MetadataEntries.Select(x => x.Artist).ToHashSet());
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
