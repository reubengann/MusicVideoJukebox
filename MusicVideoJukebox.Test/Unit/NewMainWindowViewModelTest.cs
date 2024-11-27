using MusicVideoJukebox.Core;
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
        FakeMetadataManager metadataManager;
        FakeDialogService dialogService;

        public NewMainWindowViewModelTest()
        {
            dialogService = new FakeDialogService(); 
            metadataManager = new FakeMetadataManager();
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
            navigationService.ViewModelsToGenerate[typeof(LibraryViewModel)] = new LibraryViewModel(librarySetRepo, windowLauncher, metadataManager, dialogService);
            dut.NavigateLibraryCommand.Execute(null);
            Assert.True(dut.IsLibrarySelected);
        }

        [Fact]
        public void IfSelectedLibraryAgainDisableIt()
        {
            navigationService.ViewModelsToGenerate[typeof(LibraryViewModel)] = new LibraryViewModel(librarySetRepo, windowLauncher, metadataManager, dialogService);
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
            navigationService.ViewModelsToGenerate[typeof(MetadataEditViewModel)] = new MetadataEditViewModel();
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
            navigationService.ViewModelsToGenerate[typeof(LibraryViewModel)] = new LibraryViewModel(librarySetRepo, windowLauncher, metadataManager, dialogService);
            dut.NavigateLibraryCommand.Execute(null);
            Assert.False(interfaceFader.FadingEnabled);
        }

        [Fact]
        public void FadeWhenExitedLibraryView()
        {
            navigationService.ViewModelsToGenerate[typeof(LibraryViewModel)] = new LibraryViewModel(librarySetRepo, windowLauncher, metadataManager, dialogService);
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
            navigationService.ViewModelsToGenerate[typeof(MetadataEditViewModel)] = new MetadataEditViewModel();
            dut.NavigateMetadataCommand.Execute(null);
            Assert.False(interfaceFader.FadingEnabled);
        }
    }
}
