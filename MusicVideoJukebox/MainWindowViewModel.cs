using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace MusicVideoJukebox
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MainWindowViewModel : BaseViewModel
    {
        private readonly IMediaPlayer mediaPlayer;
        DispatcherTimer progressUpdateTimer;
        DispatcherTimer scrubDebouceTimer;
        DispatcherTimer fadeTimer;
        bool isScrubbing = false;
        bool scrubbedRecently = false;
        bool isFullScreen = false;
        bool isPlaying = false;
        int currentVideoIndex = 0;
        readonly string videoFolder = "E:\\Videos\\Music Videos\\On Media Center";
        List<string> videoPaths = new List<string>();
        public ObservableCollection<string> VideoFiles { get; set; }

        public MainWindowViewModel(IMediaPlayer mediaPlayer)
        {
            this.mediaPlayer = mediaPlayer;
            mediaPlayer.Volume = 1;
            videoPaths = new List<string>(Directory.EnumerateFiles(videoFolder));
            VideoFiles = new ObservableCollection<string>(videoPaths.Select(x => Path.GetFileNameWithoutExtension(x)));

            mediaPlayer.SetSource(new System.Uri(videoPaths[currentVideoIndex]));
            // show first frame
            mediaPlayer.Play();
            isPlaying = true;
            //mediaPlayer.Pause();

            progressUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            scrubDebouceTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            progressUpdateTimer.Tick += Timer_Tick;
            scrubDebouceTimer.Tick += ScrubDebouceTimer_Tick;
            fadeTimer = new DispatcherTimer();
            fadeTimer.Interval = TimeSpan.FromSeconds(2); // Adjust the interval as needed
            fadeTimer.Tick += FadeTimer_Tick;
        }

        public string CurrentVideo
        {
            get
            {
                return VideoFiles[currentVideoIndex];
            }
            set
            {
                int selectedIndex = VideoFiles.IndexOf(value);
                if (selectedIndex < 0) return;
                currentVideoIndex = selectedIndex;
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
            OnPropertyChanged(nameof(VideoPositionTime));
            OnPropertyChanged(nameof(VideoLengthSeconds));
        }

        public ICommand PlayCommand => new DelegateCommand(PlayVideo);
        public ICommand PauseCommand => new DelegateCommand(PauseVideo);
        public ICommand StopCommand => new DelegateCommand(StopVideo);
        public ICommand NextCommand => new DelegateCommand(NextTrack);
        public ICommand PrevCommand => new DelegateCommand(PrevTrack);

        private void PrevTrack()
        {
            currentVideoIndex--;
            if (currentVideoIndex < 0) currentVideoIndex = videoPaths.Count - 1;
            PlayVideoAtCurrentIndex();
        }

        private void NextTrack()
        {
            currentVideoIndex++;
            if (currentVideoIndex > videoPaths.Count - 1) currentVideoIndex = 0;
            PlayVideoAtCurrentIndex();
        }

        private void PlayVideoAtCurrentIndex()
        {
            mediaPlayer.SetSource(new System.Uri(videoPaths[currentVideoIndex]));
            // show first frame
            mediaPlayer.Play();
            if (!isPlaying)
                mediaPlayer.Pause();
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
        }
        private void PauseVideo()
        {
            isPlaying = false;
            mediaPlayer.Pause();
            progressUpdateTimer.Stop();
        }

        private void StopVideo()
        {
            isPlaying = false;
            mediaPlayer.Stop();
            progressUpdateTimer.Stop();
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
