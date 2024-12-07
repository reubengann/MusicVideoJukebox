using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class MetadataEditViewModelTest
    {
        MetadataEditViewModel dut;
        FakeMetadataManagerFactory metadataManagerFactory;
        LibraryStore libraryStore;
        FakeDialogService dialogService;

        public MetadataEditViewModelTest()
        {
            dialogService = new FakeDialogService();
            libraryStore = new LibraryStore { FolderPath = "something" };
            metadataManagerFactory = new FakeMetadataManagerFactory();
            dut = new MetadataEditViewModel(metadataManagerFactory, libraryStore, dialogService);
        }

        [Fact]
        public async Task OnInitializePopulateTable() 
        {
            libraryStore.FolderPath = "";
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2", Status = MetadataStatus.NotDone });
            await dut.Initialize();
            Assert.Equal(2, dut.MetadataEntries.Count);
            Assert.False(dut.SaveChangesCommand.CanExecute());
        }

        [Fact]
        public async Task SaveChangedOnes()
        {
            libraryStore.FolderPath = "";
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2", Status = MetadataStatus.NotDone });
            await dut.Initialize();
            Assert.False(dut.SaveChangesCommand.CanExecute());
            dut.MetadataEntries[0].Title = "artist1changed";
            Assert.True(dut.SaveChangesCommand.CanExecute());
            Assert.True(dut.MetadataEntries[0].IsModified);
            dut.SaveChangesCommand.Execute();
            Assert.False(dialogService.ShowedError);
            Assert.Single(metadataManagerFactory.ToReturn.MetadataEntriesUpdated);
            Assert.False(dut.MetadataEntries[0].IsModified);
            Assert.False(dut.SaveChangesCommand.CanExecute());
        }

        [Fact]
        public async Task WhenFetchingMetadataPutsNotFound()
        {
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2", Status = MetadataStatus.NotDone });
            await dut.Initialize();
            dut.FetchMetadataCommand.Execute(null);
            Assert.True(dut.MetadataEntries.All(x => x.Status == MetadataStatus.NotFound));
        }

        [Fact]
        public async Task WhenFoundFillTheData()
        {
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.ReferenceDataToGet["artist1 title1"] = new GetAlbumYearResult { Success = true, AlbumTitle = "album1", ReleaseYear = 1901 };
            metadataManagerFactory.ToReturn.ReferenceDataToGet["artist2 title2"] = new GetAlbumYearResult { Success = true, AlbumTitle = "album2", ReleaseYear = 1902 };
            await dut.Initialize();
            dut.FetchMetadataCommand.Execute(null);
            Assert.True(dut.MetadataEntries.All(x => x.Status == MetadataStatus.Done));
            Assert.Equal("album1", dut.MetadataEntries[0].Album);
            Assert.True(dut.MetadataEntries.All(x => x.IsModified));
        }
    }
}
