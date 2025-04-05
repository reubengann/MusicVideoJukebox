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
        public async Task OnInitializePopulateTable() 
        {
            await libraryStore.SetLibrary(1, "");
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2", Status = MetadataStatus.NotDone });
            await dut.Initialize();
            Assert.Equal(2, dut.MetadataEntries.Count);
            Assert.False(dut.SaveChangesCommand.CanExecute());
        }

        [Fact]
        public async Task SaveChangedOnes()
        {
            await libraryStore.SetLibrary(1, "");
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2", Status = MetadataStatus.NotDone });
            await dut.Initialize();
            Assert.False(dut.SaveChangesCommand.CanExecute());
            dut.MetadataEntries[0].Title = "artist1changed";
            Assert.True(dut.SaveChangesCommand.CanExecute());
            Assert.True(dut.MetadataEntries[0].IsModified);
            dut.SaveChangesCommand.Execute();
            Assert.False(dialogService.ShowedError);
            Assert.Single(metadataManagerFactory.ToReturn.ConcreteVideoRepo.UpdatedEntries);
            Assert.False(dut.MetadataEntries[0].IsModified);
            Assert.False(dut.SaveChangesCommand.CanExecute());
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
            Assert.Single(dut.MetadataEntries);
        }

        [Fact]
        public async Task LaunchesDialogOnRightClickSelect()
        {
            await libraryStore.SetLibrary(1, "something");
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist1", Filename = "filename1", Title = "title1", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.ConcreteVideoRepo.MetadataEntries.Add(new VideoMetadata { Artist = "artist2", Filename = "filename2", Title = "title2", Status = MetadataStatus.NotDone });
            metadataManagerFactory.ToReturn.SayChangesWereMade = true;
            await dut.Initialize();
            dut.SelectedItem = dut.MetadataEntries[0];
            dut.LaunchMatchDialogCommand.Execute(null);
            Assert.True(dialogService.ShowedMatchDialog);
        }
    }
}
