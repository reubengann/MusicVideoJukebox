using MusicVideoJukebox.Core;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace MusicVideoJukebox
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
        readonly DispatcherTimer progressUpdateTimer;
        readonly DispatcherTimer scrubDebouceTimer;
        readonly DispatcherTimer fadeTimer;

        bool isScrubbing = false;
        bool scrubbedRecently = false;
        bool isFullScreen = false;
        // TODO: Redundant with ShowPlay???
        bool isPlaying = false;
        bool infoDisplayed = false;
        int currentVideoIndex = 0;
        private bool showPlay;
        private int selectedPlaylistIndex = 0;
        readonly VideoLibraryStore libraryStore;
        private readonly ISettingsWindowFactory settingsDialogFactory;

        public ObservableCollection<string> VideoFiles { get; set; }
        public ObservableCollection<string> PlaylistNames { get; set; }

        public VideoInfoViewModel InfoViewModel
        {
            get
            {
                return new VideoInfoViewModel(libraryStore.VideoLibrary.VideoIdToInfoMap[CurrentSongId]);
            }
        }

        public MainWindowViewModel(IMediaPlayer mediaPlayer, VideoLibraryStore videoLibraryStore, ISettingsWindowFactory settingsDialogFactory)
        {
            this.mediaPlayer = mediaPlayer;
            mediaPlayer.Volume = 1;
            libraryStore = videoLibraryStore;
            this.settingsDialogFactory = settingsDialogFactory;
            VideoFiles = new ObservableCollection<string>(GetNiceNames(libraryStore.VideoLibrary, SelectedPlaylistIndex));
            PlaylistNames = new ObservableCollection<string>(libraryStore.VideoLibrary.Playlists.Select(x => x.PlaylistName));
            mediaPlayer.SetSource(new System.Uri(CurrentFileName));

            progressUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            scrubDebouceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            progressUpdateTimer.Tick += Timer_Tick;
            scrubDebouceTimer.Tick += ScrubDebouceTimer_Tick;
            fadeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();
            PlayVideo();
        }

        private static IEnumerable<string> GetNiceNames(VideoLibrary library, int selectedIndex)
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

        private void ScrubDebouceTimer_Tick(object? sender, EventArgs e)
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
            scrubDebouceTimer.Stop();
            scrubbedRecently = false;
            isScrubbing = false;
        }

        public void StartScrubbing()
        {
            scrubDebouceTimer.Start();
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
