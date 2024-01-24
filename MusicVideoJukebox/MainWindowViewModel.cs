using MusicVideoJukebox.Core;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        readonly VideoLibraryStore libraryStore;
        private readonly ISettingsWindowFactory settingsDialogFactory;

        public ObservableCollection<string> VideoFiles { get; set; }

        public VideoInfoViewModel InfoViewModel
        {
            get { return new VideoInfoViewModel(libraryStore.VideoLibrary.InfoMap[libraryStore.VideoLibrary.FilePaths[currentVideoIndex]]); }
        }


        public MainWindowViewModel(IMediaPlayer mediaPlayer, VideoLibraryStore videoLibraryStore, ISettingsWindowFactory settingsDialogFactory)
        {
            this.mediaPlayer = mediaPlayer;
            mediaPlayer.Volume = 1;
            libraryStore = videoLibraryStore;
            this.settingsDialogFactory = settingsDialogFactory;
            VideoFiles = new ObservableCollection<string>(GetNiceNames(libraryStore.VideoLibrary));

            mediaPlayer.SetSource(new System.Uri(libraryStore.VideoLibrary.FilePaths[currentVideoIndex]));

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

        private static IEnumerable<string> GetNiceNames(VideoLibrary library)
        {
            var niceStrings = new List<string>();
            foreach (var path in library.FilePaths)
            {
                var vid = library.InfoMap[path];
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
                currentVideoIndex = value;
                PlayVideoAtCurrentIndex();
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
            // reset the library

        }

        private void PrevTrack()
        {
            currentVideoIndex--;
            if (currentVideoIndex < 0) currentVideoIndex = libraryStore.VideoLibrary.FilePaths.Count - 1;
            PlayVideoAtCurrentIndex();
        }

        private void NextTrack()
        {
            currentVideoIndex++;
            if (currentVideoIndex > libraryStore.VideoLibrary.FilePaths.Count - 1) currentVideoIndex = 0;
            PlayVideoAtCurrentIndex();
        }

        private void PlayVideoAtCurrentIndex()
        {
            mediaPlayer.SetSource(new System.Uri(libraryStore.VideoLibrary.FilePaths[currentVideoIndex]));
            // show first frame
            mediaPlayer.Play();
            if (!isPlaying)
                mediaPlayer.Pause();
            OnPropertyChanged(nameof(InfoViewModel));
            OnPropertyChanged(nameof(SelectedIndex));
            if (infoDisplayed) mediaPlayer.HideInfo();
            infoDisplayed = false;
            ShowPlay = false;
        }

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
