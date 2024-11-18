using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MusicVideoJukebox.Core
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IMediaPlayer mediaPlayer;
        private readonly IDialogService dialogService;
        private readonly IUIThreadTimerFactory uIThreadTimerFactory;
        private readonly IFileSystemService fileSystemService;
        private readonly IVideoLibraryBuilder videoLibraryBuilder;
        private AppSettingsStore appSettingsStore = null!;
        private IUIThreadTimer progressUpdateTimer = null!;
        private IUIThreadTimer scrubDebounceTimer = null!;
        private IUIThreadTimer fadeTimer = null!;

        bool isScrubbing = false;
        bool scrubbedRecently = false;
        bool isFullScreen = false;
        // TODO: Redundant with ShowPlay???
        bool isPlaying = false;
        bool infoDisplayed = false;
        int currentVideoIndex = 0;
        private bool showPlay;
        private int selectedPlaylistIndex = 0;
        VideoLibraryStore libraryStore = null!;
        private readonly ISettingsWindowFactory settingsDialogFactory;

        public ObservableCollection<string> VideoFiles { get; set; } = null!;
        public ObservableCollection<string> PlaylistNames { get; set; } = null!;

        public VideoInfoViewModel InfoViewModel
        {
            get
            {
                return new VideoInfoViewModel(libraryStore.VideoLibrary.VideoIdToInfoMap[CurrentSongId]);
            }
        }

        public MainWindowViewModel(IMediaPlayer mediaPlayer, ISettingsWindowFactory settingsDialogFactory, IDialogService dialogService, IUIThreadTimerFactory uIThreadTimerFactory, IFileSystemService fileSystemService,
            IVideoLibraryBuilder videoLibraryBuilder)
        {
            this.mediaPlayer = mediaPlayer;
            this.dialogService = dialogService;
            this.uIThreadTimerFactory = uIThreadTimerFactory;
            this.fileSystemService = fileSystemService;
            this.videoLibraryBuilder = videoLibraryBuilder;
            this.settingsDialogFactory = settingsDialogFactory;
            mediaPlayer.Volume = 1;
        }

        public async Task Initialize()
        {
            appSettingsStore = await AppSettingsStore.Create();

            if (string.IsNullOrWhiteSpace(appSettingsStore.VideoLibraryPath) || !fileSystemService.FolderExists(appSettingsStore.VideoLibraryPath))
            {
                var result = dialogService.ShowFolderSelect("Select a folder for the Video Library", fileSystemService.GetMyDocuments());
                if (result.Accepted)
                {
                    ArgumentNullException.ThrowIfNull(result.SelectedFolder);
                    appSettingsStore.UpdateVideoLibraryPath(result.SelectedFolder);
                    await appSettingsStore.Save();
                }
                else
                {
                    dialogService.ShowError("A folder is required to continue. The application will now exit.");
                    dialogService.ShutDownApp();
                }
            }
            ArgumentNullException.ThrowIfNull(appSettingsStore.VideoLibraryPath);
            libraryStore = new VideoLibraryStore(await videoLibraryBuilder.BuildAsync(appSettingsStore.VideoLibraryPath));

            PlaylistNames = new ObservableCollection<string>(libraryStore.VideoLibrary.Playlists.Select(x => x.PlaylistName));

            // figure out the SelectedPlaylistIndex
            var previousPlaylist = libraryStore.VideoLibrary.Playlists.FirstOrDefault(x => x.PlaylistId == libraryStore.VideoLibrary.ProgressPersister.CurrentPlayStatus.playlist_id);
            if (previousPlaylist != null)
            {
                var previousPlaylistIndex = libraryStore.VideoLibrary.Playlists.IndexOf(previousPlaylist);
                if (previousPlaylistIndex != -1)
                {
                    selectedPlaylistIndex = previousPlaylistIndex;
                }
                // figure out the SelectedIndex
                var previousSong = libraryStore.VideoLibrary.PlaylistIdToSongOrderMap[previousPlaylist.PlaylistId].FirstOrDefault(x => x.Info.VideoId == libraryStore.VideoLibrary.ProgressPersister.CurrentPlayStatus.song_id);
                if (previousSong != null)
                {
                    if (previousSong.PlayOrder < libraryStore.VideoLibrary.PlaylistIdToSongMap[previousPlaylist.PlaylistId].Count)
                        currentVideoIndex = previousSong.PlayOrder - 1;
                }
            }

            VideoFiles = new ObservableCollection<string>(GetNiceNames(libraryStore.VideoLibrary, SelectedPlaylistIndex));
            mediaPlayer.SetSource(new System.Uri(CurrentFileName));

            progressUpdateTimer = uIThreadTimerFactory.Create(TimeSpan.FromMilliseconds(500));
            scrubDebounceTimer = uIThreadTimerFactory.Create(TimeSpan.FromMilliseconds(500));
            progressUpdateTimer.Tick += Timer_Tick;
            scrubDebounceTimer.Tick += ScrubDebounceTimer_Tick;
            fadeTimer = uIThreadTimerFactory.Create(TimeSpan.FromSeconds(2));
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();
            PlayVideo();
        }

        private static List<string> GetNiceNames(VideoLibrary library, int selectedIndex)
        {
            var niceStrings = new List<string>();
            var playlistId = library.Playlists[selectedIndex].PlaylistId;
            var songIds = library.PlaylistIdToSongMap[playlistId];
            foreach (var songId in songIds)
            {
                var vid = library.VideoIdToInfoMap[songId];
                if (vid.Artist == null)
                    niceStrings.Add(vid.Title);
                else
                    niceStrings.Add($"{vid.Artist} - {vid.Title}");
            }
            return niceStrings;
        }

        public int SelectedIndex
        {
            get { return currentVideoIndex; }
            set
            {
                // When loading a new playlist, this ends up being -1. Ignore that event.
                if (value < 0)
                    return;
                currentVideoIndex = value;
                PlayVideoAtCurrentIndex();
            }
        }

        public int SelectedPlaylistIndex
        {
            get => selectedPlaylistIndex;
            set
            {
                selectedPlaylistIndex = value;

                VideoFiles = new ObservableCollection<string>(GetNiceNames(libraryStore.VideoLibrary, selectedPlaylistIndex));
                OnPropertyChanged(nameof(VideoFiles));
                currentVideoIndex = 0;
                PlayVideoAtCurrentIndex();
                // TODO: Because we got to this from an event, updating the currentVideoIndex will not update the highlighted item in the listview.
            }
        }


        private void FadeTimer_Tick(object? sender, EventArgs e)
        {
            mediaPlayer.FadeButtonsOut();
        }

        private void ScrubDebounceTimer_Tick(object? sender, EventArgs e)
        {
            scrubbedRecently = false;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (isScrubbing) return;
            if (VideoPositionTime > 3 && !infoDisplayed)
            {
                mediaPlayer.ShowInfo();
                infoDisplayed = true;
            }
            if (VideoPositionTime > 10 && infoDisplayed)
            {
                mediaPlayer.HideInfo();
                infoDisplayed = false;
            }
            OnPropertyChanged(nameof(VideoPositionTime));
            OnPropertyChanged(nameof(VideoLengthSeconds));
        }

        public ICommand PlayCommand => new DelegateCommand(PlayVideo);
        public ICommand PauseCommand => new DelegateCommand(PauseVideo);
        public ICommand PlayPauseCommand => new DelegateCommand(PlayPause);

        private void PlayPause()
        {
            if (isPlaying)
                PauseVideo();
            else PlayVideo();
        }

        public ICommand NextCommand => new DelegateCommand(NextTrack);
        public ICommand PrevCommand => new DelegateCommand(PrevTrack);
        public ICommand SettingsCommand => new DelegateCommand(OpenSettings);
        public ICommand ExitFullScreenCommand => new DelegateCommand(ExitFullScreenMaybe);

        private void ExitFullScreenMaybe()
        {
            if (isFullScreen)
            {
                mediaPlayer.SetWindowed();
                isFullScreen = false;
            }
        }

        public bool ShowPlay
        {
            get => showPlay;
            set
            {
                showPlay = value;
                OnPropertyChanged(nameof(ShowPlay));
                OnPropertyChanged(nameof(ShowPause));
            }
        }
        public bool ShowPause => !ShowPlay;

        private void OpenSettings()
        {
            if (isPlaying)
                PauseVideo();
            var settingsDialog = settingsDialogFactory.Create(libraryStore);
            settingsDialog.ShowDialog();
            // reset the current playlist maybe
        }

        private void PrevTrack()
        {
            currentVideoIndex--;
            if (currentVideoIndex < 0) currentVideoIndex = CurrentSongIds.Count - 1;
            PlayVideoAtCurrentIndex();
        }

        private void NextTrack()
        {
            currentVideoIndex++;
            if (currentVideoIndex > CurrentSongIds.Count - 1) currentVideoIndex = 0;
            PlayVideoAtCurrentIndex();
        }

        private void PlayVideoAtCurrentIndex()
        {
            mediaPlayer.SetSource(new System.Uri(CurrentFileName));
            // show first frame if paused
            mediaPlayer.Play();
            if (!isPlaying)
                mediaPlayer.Pause();
            OnPropertyChanged(nameof(InfoViewModel));
            OnPropertyChanged(nameof(SelectedIndex));
            if (infoDisplayed) mediaPlayer.HideInfo();
            infoDisplayed = false;
            ShowPlay = false;
            // fire and forget the update to the current persistance
            _ = libraryStore.VideoLibrary.ProgressPersister.StoreStatusAsync(CurrentPlaylist.PlaylistId, CurrentSongId);
        }

        private string CurrentFileName => libraryStore.VideoLibrary.FilePaths[CurrentSongId];
        private Playlist CurrentPlaylist => libraryStore.VideoLibrary.Playlists[SelectedPlaylistIndex];
        private int CurrentSongId => CurrentSongIds[currentVideoIndex];
        private List<int> CurrentSongIds => libraryStore.VideoLibrary.PlaylistIdToSongMap[CurrentPlaylist.PlaylistId];

        public double Volume
        {
            get => mediaPlayer.Volume;
            set => mediaPlayer.Volume = value;
        }

        public void StopScrubbing()
        {
            scrubDebounceTimer.Stop();
            scrubbedRecently = false;
            isScrubbing = false;
        }

        public void StartScrubbing()
        {
            scrubDebounceTimer.Start();
            isScrubbing = true;
        }

        private void PlayVideo()
        {
            isPlaying = true;
            mediaPlayer.Play();
            progressUpdateTimer.Start();
            OnPropertyChanged(nameof(VideoLengthSeconds));
            OnPropertyChanged(nameof(VideoPositionTime));
            ShowPlay = false;
        }

        private void PauseVideo()
        {
            isPlaying = false;
            mediaPlayer.Pause();
            progressUpdateTimer.Stop();
            ShowPlay = true;
        }

        public void ChangeToFullScreenToggled()
        {
            if (isFullScreen)
            {
                mediaPlayer.SetWindowed();
                isFullScreen = false;
            }
            else
            {
                mediaPlayer.SetFullScreen();
                isFullScreen = true;
            }
        }

        public void UserInteracted()
        {
            fadeTimer.Stop();
            fadeTimer.Start();
            mediaPlayer.MaybeFadeButtonsIn();
        }

        public void DonePlaying()
        {
            NextTrack();
        }

        public double VideoLengthSeconds => mediaPlayer.LengthSeconds;
        public double VideoPositionTime
        {
            get => mediaPlayer.CurrentTimeSeconds;
            set
            {
                if (scrubbedRecently) return;
                scrubbedRecently = true;
                mediaPlayer.CurrentTimeSeconds = value;
            }
        }
    }
}
