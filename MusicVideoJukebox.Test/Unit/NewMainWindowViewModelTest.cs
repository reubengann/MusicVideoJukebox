using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class NewMainWindowViewModelTest
    {
        FakeNavigationService navigationService;
        MainWindowViewModel dut;
        FakeInterfaceFader interfaceFader;
        FakeLibrarySetRepo librarySetRepo;
        FakeWindowLauncher windowLauncher;
        FakeMetadataManagerFactory metadataManagerFactory;
        FakeDialogService dialogService;
        FakeVideoRepo videoRepo;
        LibraryStore libraryStore;

        public NewMainWindowViewModelTest()
        {
            libraryStore = new LibraryStore();
            videoRepo = new FakeVideoRepo();
            dialogService = new FakeDialogService(); 
            metadataManagerFactory = new FakeMetadataManagerFactory();
            librarySetRepo = new FakeLibrarySetRepo();
            interfaceFader = new FakeInterfaceFader();
            navigationService = new FakeNavigationService();
            windowLauncher = new FakeWindowLauncher();

            dut = new MainWindowViewModel(navigationService, libraryStore);
            dut.Initialize(interfaceFader);
        }

        [Fact]
        public void NothingIsSelectedInitially()
        {
            Assert.False(dut.IsLibrarySelected);
            Assert.False(dut.IsPlaylistEditSelected);
            Assert.False(dut.IsMetadataSelected);
        }

        [Fact]
        public void IfSelectedLibraryEnableIt()
        {
            navigationService.ViewModelsToGenerate[typeof(LibraryViewModel)] = new LibraryViewModel(libraryStore, librarySetRepo, windowLauncher, metadataManagerFactory, dialogService, navigationService);
            dut.NavigateLibraryCommand.Execute(null);
            Assert.True(dut.IsLibrarySelected);
        }

        [Fact]
        public void IfSelectedLibraryAgainDisableIt()
        {
            navigationService.ViewModelsToGenerate[typeof(LibraryViewModel)] = new LibraryViewModel(libraryStore, librarySetRepo, windowLauncher, metadataManagerFactory, dialogService, navigationService);
            dut.NavigateLibraryCommand.Execute(null);
            Assert.True(dut.IsLibrarySelected);
            dut.NavigateLibraryCommand.Execute(null);
            Assert.False(dut.IsLibrarySelected);
        }

        [Fact]
        public void IfSelectedPlaylistAgainDisableIt()
        {
            navigationService.ViewModelsToGenerate[typeof(PlaylistEditViewModel)] = new PlaylistEditViewModel(libraryStore, metadataManagerFactory);
            dut.NavigatePlaylistEditCommand.Execute(null);
            Assert.True(dut.IsPlaylistEditSelected);
            dut.NavigatePlaylistEditCommand.Execute(null);
            Assert.False(dut.IsPlaylistEditSelected);
        }

        [Fact]
        public void IfSelectedMetadataAgainDisableIt()
        {
            navigationService.ViewModelsToGenerate[typeof(MetadataEditViewModel)] = new MetadataEditViewModel(metadataManagerFactory, libraryStore, dialogService);
            dut.NavigateMetadataCommand.Execute(null);
            Assert.True(dut.IsMetadataSelected);
            dut.NavigateMetadataCommand.Execute(null);
            Assert.False(dut.IsMetadataSelected);
        }

        [Fact]
        public void NoViewModelInitially()
        {
            Assert.Null(dut.CurrentViewModel);
        }
    }
}
