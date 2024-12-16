using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class VideoPlayingViewModel : BaseViewModel
    {
        private bool isPlaying = false;
        private readonly IMediaPlayer2 mediaPlayer;
        private readonly IUIThreadTimerFactory uIThreadTimerFactory;
        IUIThreadTimer scrubDebounceTimer;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly LibraryStore libraryStore;
        IMetadataManager? metadataManager;
        int? currentLibraryId;
        PlaylistNavigator? playlistNavigator;
        bool scrubbedRecently = false;
        bool isScrubbing = false;

        public double Volume
        {
            get => mediaPlayer.Volume;
            set => SetUnderlyingProperty(mediaPlayer.Volume, value, v => mediaPlayer.Volume = v);
        }

        // TEMP
        public PlaylistTrack? CurrentPlaylistTrack {  get; private set; }

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
                mediaPlayer.CurrentTimeSeconds = value;
            }
        }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        public ICommand SkipNextCommand { get; set; }
        public ICommand SkipPreviousCommand { get; set; }
        IUIThreadTimer progressUpdateTimer;

        public VideoPlayingViewModel(IMediaPlayer2 mediaElementMediaPlayer, 
            IUIThreadTimerFactory uIThreadTimerFactory, 
            IMetadataManagerFactory metadataManagerFactory,
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
            this.libraryStore = libraryStore;
            progressUpdateTimer = uIThreadTimerFactory.Create(TimeSpan.FromMilliseconds(500));
            scrubDebounceTimer = uIThreadTimerFactory.Create(TimeSpan.FromMilliseconds(500));
            progressUpdateTimer.Tick += ProgressTimerTick;
            scrubDebounceTimer.Tick += ScrubDebounceTimer_Tick;

            // TEMP
            Volume = 1;
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
            SetSource(playlistNavigator.Previous());
        }

        private void SkipNext()
        {
            if (playlistNavigator == null) return;
            SetSource(playlistNavigator.Next());
        }

        private void LibraryStore_LibraryChanged()
        {
            if (libraryStore.FolderPath == null) return;
            metadataManager = metadataManagerFactory.Create(libraryStore.FolderPath);

        }

        private void ProgressTimerTick(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(VideoPositionTime));
            OnPropertyChanged(nameof(VideoLengthSeconds));
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

        private void SetSource(PlaylistTrack track)
        {
            if (libraryStore.FolderPath == null) return;
            CurrentPlaylistTrack = track;
            mediaPlayer.SetSource(new Uri(Path.Combine(libraryStore.FolderPath, track.FileName)));
        }

        public async Task Recheck()
        {
            if (libraryStore.LibraryId == currentLibraryId || libraryStore.FolderPath == null) return;

            metadataManager = metadataManagerFactory.Create(libraryStore.FolderPath);
            var playlists = await metadataManager.GetPlaylists();
            if (playlists.Count == 0)
            {
                mediaPlayer.Stop();
                IsPlaying = false;
                return;
            }

            //TEMP
            var firstPlaylist = playlists.Where(x => x.IsAll).First();
            var tracks = await metadataManager.GetPlaylistTracks(firstPlaylist.PlaylistId);
            if (tracks.Count == 0) return;
            playlistNavigator = new PlaylistNavigator(tracks);

            SetSource(playlistNavigator.CurrentTrack);
            Play();
        }
    }
}
