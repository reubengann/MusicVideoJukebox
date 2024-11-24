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
            //dut = new MainWindowViewModel(settingsDialogFactory, dialogService, uIThreadFactory, fileSystemService, videoLibraryBuilder, appSettingsFactory);
        }

        [Fact]
        public async Task PlaysOnInitialized()
        {
            appSettingsFactory.Settings.VideoLibraryPath = "avalidpath";
            fileSystemService.ExistingPaths.Add("avalidpath");
            WithPlaylist(1);
            WithOneSong(1, 1);
            await dut.Initialize(mediaPlayer);
            Assert.False(dialogService.ShowedFolderSelect);
            Assert.True(mediaPlayer.SetToPlay);
            Assert.False(dut.ShowPlay);
        }

        [Fact]
        public async Task PromptsWhenNoLibraryLoaded()
        {
            dialogService.PickResultToReturn.SelectedFolder = "avalidpath";
            dialogService.PickResultToReturn.Accepted = true;
            WithPlaylist(1);
            WithOneSong(1, 1);
            await dut.Initialize(mediaPlayer);
            Assert.True(dialogService.ShowedFolderSelect);
        }

        private void WithPlaylist(int id)
        {
            videoLibraryBuilder.ToReturn.Playlists.Add(new Playlist { PlaylistId = id });
            videoLibraryBuilder.ToReturn.PlaylistIdToSongMap[id] = [];
            videoLibraryBuilder.ToReturn.PlaylistIdToSongOrderMap[id] = [];
        }

        private void WithOneSong(int playlistId, int songId)
        {
            videoLibraryBuilder.ToReturn.PlaylistIdToSongMap[playlistId].Add(1);
            videoLibraryBuilder.ToReturn.VideoIdToInfoMap[songId] = new VideoInfo { };
            videoLibraryBuilder.ToReturn.FilePaths[songId] = @"c:\temp\foo";
        }
    }
}
