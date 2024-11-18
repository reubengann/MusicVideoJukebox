using MusicVideoJukebox.Core;

namespace MusicVideoJukebox.Test
{
    public class MainWindowViewModelTest
    {
        MainWindowViewModel dut;
        FakeMediaPlayer mediaPlayer;
        FakeSettingsWindowFactory settingsDialogFactory;
        FakeDialogService dialogService;
        FakeUIThreadFactory uIThreadFactory;
        FakeFileSystemService fileSystemService;
        FakeVideoLibraryBuilder videoLibraryBuilder;
        FakeAppSettingsFactory appSettingsFactory;

        public MainWindowViewModelTest()
        {
            mediaPlayer = new FakeMediaPlayer();
            settingsDialogFactory = new FakeSettingsWindowFactory();
            dialogService = new FakeDialogService();
            fileSystemService = new FakeFileSystemService();
            videoLibraryBuilder = new FakeVideoLibraryBuilder();
            uIThreadFactory = new FakeUIThreadFactory();
            appSettingsFactory = new FakeAppSettingsFactory();
            dut = new MainWindowViewModel(mediaPlayer, settingsDialogFactory, dialogService, uIThreadFactory, fileSystemService, videoLibraryBuilder, appSettingsFactory);
        }

        [Fact]
        public async Task Foo()
        {
            WithOneSong();
            await dut.Initialize();
        }

        private void WithOneSong()
        {
            videoLibraryBuilder.ToReturn.Playlists.Add(new Playlist { PlaylistId = 1 });
            videoLibraryBuilder.ToReturn.PlaylistIdToSongOrderMap[1] = [];
            videoLibraryBuilder.ToReturn.PlaylistIdToSongMap[1] = [1];
            videoLibraryBuilder.ToReturn.VideoIdToInfoMap[1] = new VideoInfo { };
            videoLibraryBuilder.ToReturn.FilePaths[1] = @"c:\temp\foo";
        }
    }
}
