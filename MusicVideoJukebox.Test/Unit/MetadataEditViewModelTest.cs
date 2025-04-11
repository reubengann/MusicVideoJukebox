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
        FakeLibrarySetRepo librarySetRepo;

        public MetadataEditViewModelTest()
        {
            librarySetRepo = new FakeLibrarySetRepo();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            libraryStore = new LibraryStore(librarySetRepo, metadataManagerFactory);
            dialogService = new FakeDialogService();
            dut = new MetadataEditViewModel(metadataManagerFactory, libraryStore, dialogService);
        }

        [Fact]
        public async Task WhenResyncingReloadTheTable()
        {
            await libraryStore.SetLibrary(1, "something");
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.SayChangesWereMade = true;
            await dut.Initialize();
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.RemoveAt(1);
            dut.RefreshDatabaseCommand.Execute(null);
            Assert.Single(dut.FilteredMetadataEntries);
        }

        [Fact]
        public async Task LaunchesDialogOnRightClickSelect()
        {
            await libraryStore.SetLibrary(1, "something");
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.SayChangesWereMade = true;
            await dut.Initialize();
            dut.SelectedItem = dut.FilteredMetadataEntries[0];
            dut.EditMetadataCommand.Execute(null);
            Assert.True(dialogService.ShowedMatchDialog);
        }
    }
}
