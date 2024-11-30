using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.ViewModels;
using MusicVideoJukebox.Test.Fakes;

namespace MusicVideoJukebox.Test.Unit
{
    public class NewMainWindowViewModelTest
    {
        FakeNavigationService navigationService;
        NewMainWindowViewModel dut;
        FakeInterfaceFader interfaceFader;
        FakeLibrarySetRepo librarySetRepo;
        FakeWindowLauncher windowLauncher;
        FakeMetadataManagerFactory metadataManagerFactory;
        FakeDialogService dialogService;
        FakeVideoRepo videoRepo;
        FakeMetadataProvider metadataProvider;
        LibraryStore libraryStore;

        public NewMainWindowViewModelTest()
        {
            libraryStore = new LibraryStore();
            videoRepo = new FakeVideoRepo();
            metadataProvider = new FakeMetadataProvider();
            dialogService = new FakeDialogService(); 
            metadataManagerFactory = new FakeMetadataManagerFactory();
            librarySetRepo = new FakeLibrarySetRepo();
            interfaceFader = new FakeInterfaceFader();
            navigationService = new FakeNavigationService();
            windowLauncher = new FakeWindowLauncher();

            dut = new NewMainWindowViewModel(navigationService);
            dut.Initialize(interfaceFader);
        }

        [Fact]
        public void NothingIsSelectedInitially()
        {
            Assert.False(dut.IsLibrarySelected);
            Assert.False(dut.IsPlaylistSelected);
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
            navigationService.ViewModelsToGenerate[typeof(NewPlaylistViewModel)] = new NewPlaylistViewModel();
            dut.NavigatePlaylistCommand.Execute(null);
            Assert.True(dut.IsPlaylistSelected);
            dut.NavigatePlaylistCommand.Execute(null);
            Assert.False(dut.IsPlaylistSelected);
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

        [Fact]
        public void DontFadeWhenInLibraryView()
        {
            navigationService.ViewModelsToGenerate[typeof(LibraryViewModel)] = new LibraryViewModel(libraryStore, librarySetRepo, windowLauncher, metadataManagerFactory, dialogService, navigationService);
            dut.NavigateLibraryCommand.Execute(null);
            Assert.False(interfaceFader.FadingEnabled);
        }

        [Fact]
        public void FadeWhenExitedLibraryView()
        {
            navigationService.ViewModelsToGenerate[typeof(LibraryViewModel)] = new LibraryViewModel(libraryStore, librarySetRepo, windowLauncher, metadataManagerFactory, dialogService, navigationService);
            dut.NavigateLibraryCommand.Execute(null);
            dut.NavigateLibraryCommand.Execute(null);
            Assert.True(interfaceFader.FadingEnabled);
        }

        [Fact]
        public void DontFadeWhenInPlaylistView()
        {
            navigationService.ViewModelsToGenerate[typeof(NewPlaylistViewModel)] = new NewPlaylistViewModel();
            dut.NavigatePlaylistCommand.Execute(null);
            Assert.False(interfaceFader.FadingEnabled);
        }

        [Fact]
        public void DontFadeWhenInMetadataView()
        {
            navigationService.ViewModelsToGenerate[typeof(MetadataEditViewModel)] = new MetadataEditViewModel(metadataManagerFactory, libraryStore, dialogService);
            dut.NavigateMetadataCommand.Execute(null);
            Assert.False(interfaceFader.FadingEnabled);
        }
    }
}
