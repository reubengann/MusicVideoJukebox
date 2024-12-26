using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class LibraryViewModelTest
    {
        LibraryViewModel dut;
        FakeLibrarySetRepo librarySetRepo;
        FakeWindowLauncher windowLauncher;
        FakeDialogService dialogService;
        FakeMetadataManagerFactory metadataManagerFactory;
        FakeNavigationService navigationService;
        LibraryStore libraryStore;

        public LibraryViewModelTest()
        {
            navigationService = new FakeNavigationService();
            librarySetRepo = new FakeLibrarySetRepo();
            metadataManagerFactory = new FakeMetadataManagerFactory();
            libraryStore = new LibraryStore(librarySetRepo, metadataManagerFactory);
            dialogService = new FakeDialogService();
            windowLauncher = new FakeWindowLauncher();

            dut = new LibraryViewModel(libraryStore, librarySetRepo, windowLauncher, metadataManagerFactory, dialogService, navigationService);
        }

        [Fact]
        public void WhenMetadataDoesntExistInFolderCreatesIt()
        {
            metadataManagerFactory.ToReturn.CreatedMetadataFolders.Add("existing");
            windowLauncher.ToReturn.Name = "Test";
            windowLauncher.ToReturn.Path = "nonexisting";
            windowLauncher.ToReturn.Accepted = true;
            dut.SelectLibraryCommand.Execute(new LibraryItemViewModel { LibraryId = null, IsAddNew = true });
            Assert.Contains("nonexisting", metadataManagerFactory.ToReturn.CreatedMetadataFolders);
            Assert.Single(librarySetRepo.LibraryItems);
        }

        [Fact]
        public async Task WhenCreatingRefreshTheList()
        {
            await dut.Initialize();
            metadataManagerFactory.ToReturn.CreatedMetadataFolders.Add("existing");
            windowLauncher.ToReturn.Name = "Test";
            windowLauncher.ToReturn.Path = "nonexisting";
            windowLauncher.ToReturn.Accepted = true;
            dut.SelectLibraryCommand.Execute(new LibraryItemViewModel { LibraryId = null, IsAddNew = true });
            Assert.Equal(2, dut.Items.Count);
        }

        [Fact]
        public async Task WhenSelectingSetsTheStore()
        {
            await dut.Initialize();
            dut.SelectLibraryCommand.Execute(new LibraryItemViewModel { IsAddNew = false, LibraryId = 1, LibraryItem = new LibraryItem {  LibraryId = 1, FolderPath = "foobar" } });
            Assert.Equal(1, libraryStore.CurrentState.LibraryId);
            Assert.Equal("foobar", libraryStore.CurrentState.LibraryPath);
        }

        [Fact]
        public async Task WhenSelectingNavigatesBackToNothing()
        {
            navigationService.viewModel = dut;
            await dut.Initialize();
            dut.SelectLibraryCommand.Execute(new LibraryItemViewModel { IsAddNew = false, LibraryId = 1, LibraryItem = new LibraryItem { FolderPath = "foobar" } });
            Assert.Null(navigationService.CurrentViewModel);
        }
    }
}
