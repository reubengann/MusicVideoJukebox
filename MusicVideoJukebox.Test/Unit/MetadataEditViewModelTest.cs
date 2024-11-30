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
            libraryStore = new LibraryStore();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            dut = new MetadataEditViewModel(metadataManagerFactory, libraryStore, dialogService);
        }

        [Fact]
        public async Task OnInitializePopulateTable() 
        {
            libraryStore.FolderPath = "";
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1" });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2" });
            await dut.Initialize();
            Assert.Equal(2, dut.MetadataEntries.Count);
            Assert.False(dut.SaveChangesCommand.CanExecute());
        }

        [Fact]
        public async Task SaveChangedOnes()
        {
            libraryStore.FolderPath = "";
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1" });
            metadataManagerFactory.ToReturn.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2" });
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
    }
}
