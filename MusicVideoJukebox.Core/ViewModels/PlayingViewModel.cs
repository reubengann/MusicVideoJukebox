using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.UserInterface;
using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlayingViewModel : BaseViewModel
    {
        private const int VIDEO_INFO_START_GUTTER = 3;
        private const int VIDEO_INFO_END_GUTTER = 10;
        private bool isPlaying = true;
        private readonly IMediaPlayer mediaPlayer;
        private readonly IUIThreadTimerFactory uIThreadTimerFactory;
        IUIThreadTimer scrubDebounceTimer;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly IFadesWhenInactive fadesWhenInactive;
        private readonly LibraryStore libraryStore;
        IMetadataManager? metadataManager;
        int? currentLibraryId;
        PlaylistNavigator? playlistNavigator;
        bool scrubbedRecently = false;
        bool isScrubbing = false;
        bool infoDisplayed = false;

        public VideoInfoViewModel? InfoViewModel { get; private set; }
        public double Volume
        {
            get => mediaPlayer.Volume;
            set => SetUnderlyingProperty(mediaPlayer.Volume, value, v => mediaPlayer.Volume = v);
        }

        // TEMP
        public PlaylistTrack? CurrentPlaylistTrack { get => currentPlaylistTrack; set { if (SetProperty(ref currentPlaylistTrack, value)) { OnPropertyChanged(nameof(ProgressSliderEnabled)); } } }

        public bool ProgressSliderEnabled => CurrentPlaylistTrack != null;

        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
        }

        public double VideoLengthSeconds => mediaPlayer.LengthSeconds;
        public double VideoPositionTime
        {
            get => mediaPlayer.CurrentTimeSeconds;
            set
            {
                if (scrubbedRecently) return;
                scrubbedRecently = true;
                var prevVolume = Volume;
                if (!IsPlaying)
                    Volume = 0;
                mediaPlayer.CurrentTimeSeconds = value;
                if (!IsPlaying)
                    Volume = prevVolume;
            }
        }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand SkipNextCommand { get; set; }
        public ICommand SkipPreviousCommand { get; set; }
        public bool IsSidebarVisible 
        { 
            get => sidebarVisible; 
            private set => SetProperty(ref sidebarVisible, value); 
        }

        IUIThreadTimer progressUpdateTimer;
        private PlaylistTrack? currentPlaylistTrack;
        private bool sidebarVisible;
        private int? currentPlaylistId;

        public PlayingViewModel(IMediaPlayer mediaElementMediaPlayer, 
            IUIThreadTimerFactory uIThreadTimerFactory, 
            IMetadataManagerFactory metadataManagerFactory,
            IFadesWhenInactive fadesWhenInactive,
            LibraryStore libraryStore
            )
        {
            PlayCommand = new DelegateCommand(Play);
            PauseCommand = new DelegateCommand(Pause);
            SkipNextCommand = new DelegateCommand(SkipNext);
            SkipPreviousCommand = new DelegateCommand(SkipPrevious);
            this.mediaPlayer = mediaElementMediaPlayer;
            this.uIThreadTimerFactory = uIThreadTimerFactory;
            this.metadataManagerFactory = metadataManagerFactory;
            this.fadesWhenInactive = fadesWhenInactive;
            this.libraryStore = libraryStore;
            progressUpdateTimer = uIThreadTimerFactory.Create(TimeSpan.FromMilliseconds(500));
            scrubDebounceTimer = uIThreadTimerFactory.Create(TimeSpan.FromMilliseconds(500));
            progressUpdateTimer.Tick += ProgressTimerTick;
            scrubDebounceTimer.Tick += ScrubDebounceTimer_Tick;

            fadesWhenInactive.VisibilityChanged += FadesWhenInactive_VisibilityChanged;

            // TEMP
            Volume = 1;
        }

        private void FadesWhenInactive_VisibilityChanged(object? sender, VisibilityChangedEventArgs e)
        {
            IsSidebarVisible = e.IsVisible;
        }

        private void ScrubDebounceTimer_Tick(object? sender, EventArgs e)
        {
            scrubbedRecently = false;
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

        private void SkipPrevious()
        {
            if (playlistNavigator == null) return;
            mediaPlayer.HideInfoImmediate();
            SetSource(playlistNavigator.Previous());
        }

        private void SkipNext()
        {
            if (playlistNavigator == null) return;
            mediaPlayer.HideInfoImmediate();
            SetSource(playlistNavigator.Next());
        }

        private void LibraryStore_LibraryChanged()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;
            metadataManager = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);

        }

        private void ProgressTimerTick(object? sender, EventArgs e)
        {
            if (isScrubbing) return;

            bool isInStartGutter = VideoPositionTime > VIDEO_INFO_START_GUTTER && VideoPositionTime < VIDEO_INFO_END_GUTTER;
            bool isInEndGutter = VideoPositionTime > VideoLengthSeconds - VIDEO_INFO_END_GUTTER && VideoPositionTime < VideoLengthSeconds - VIDEO_INFO_START_GUTTER;

            if (infoDisplayed && !(isInStartGutter || isInEndGutter))
            {
                mediaPlayer.FadeInfoOut();
                infoDisplayed = false;
            }
            else if (!infoDisplayed && (isInStartGutter || isInEndGutter))
            {
                mediaPlayer.FadeInfoIn();
                infoDisplayed = true;
            }

            OnPropertyChanged(nameof(VideoPositionTime));
            OnPropertyChanged(nameof(VideoLengthSeconds));
        }

        public void DonePlaying()
        {
            SkipNext();
        }

        private void Pause()
        {
            mediaPlayer.Pause();
            IsPlaying = false;
            progressUpdateTimer.Stop();
        }

        private void Play()
        {
            mediaPlayer.Play();
            IsPlaying = true;
            progressUpdateTimer.Start();
        }

        private void SetSource(PlaylistTrack? track)
        {
            mediaPlayer.HideInfoImmediate();
            if (libraryStore.CurrentState.LibraryPath == null) return;
            CurrentPlaylistTrack = track;
            if (track == null) return;
            mediaPlayer.SetSource(new Uri(Path.Combine(libraryStore.CurrentState.LibraryPath, track.FileName)));
            InfoViewModel = new VideoInfoViewModel(track);
            OnPropertyChanged(nameof(VideoPositionTime));
            OnPropertyChanged(nameof(InfoViewModel));
        }

        public async Task Recheck()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;

            var wasPlaying = IsPlaying;
            
            if (libraryStore.CurrentState.LibraryId != currentLibraryId)
            {
                currentLibraryId = libraryStore.CurrentState.LibraryId;
                metadataManager = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
                await metadataManager.EnsureCreated();
                await LoadPlaylist();
            }
            else if (libraryStore.CurrentPlaylistId != currentPlaylistId)
            {
                await LoadPlaylist();
            }
            else
            {
                ArgumentNullException.ThrowIfNull(playlistNavigator);
                var restoreStateResult = await playlistNavigator.CheckPlaylistState();
                if (restoreStateResult.NeedsToChangeTrack)
                {
                    SetSource(restoreStateResult.NewPlaylistTrack);
                }   
            }
            if (wasPlaying)
            {
                Play();
            }
        }

        async Task LoadPlaylist()
        {
            ArgumentNullException.ThrowIfNull(metadataManager);
            playlistNavigator = new PlaylistNavigator(metadataManager);
            SetSource(await playlistNavigator.Resume());
            currentPlaylistId = playlistNavigator.CurrentPlaylistId;
        }
    }
}
