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
        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly LibraryStore libraryStore;
        IMetadataManager? metadataManager;
        int? currentLibraryId;
        PlaylistNavigator? playlistNavigator;

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
                //if (scrubbedRecently) return;
                //scrubbedRecently = true;
                mediaPlayer.CurrentTimeSeconds = value;
            }
        }

        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }
        IUIThreadTimer progressUpdateTimer;

        public VideoPlayingViewModel(IMediaPlayer2 mediaElementMediaPlayer, 
            IUIThreadTimerFactory uIThreadTimerFactory, 
            IMetadataManagerFactory metadataManagerFactory,
            LibraryStore libraryStore
            )
        {
            PlayCommand = new DelegateCommand(Play);
            PauseCommand = new DelegateCommand(Pause);
            this.mediaPlayer = mediaElementMediaPlayer;
            this.uIThreadTimerFactory = uIThreadTimerFactory;
            this.metadataManagerFactory = metadataManagerFactory;
            this.libraryStore = libraryStore;
            progressUpdateTimer = uIThreadTimerFactory.Create(TimeSpan.FromMilliseconds(500));
            progressUpdateTimer.Tick += ProgressTimerTick;
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

        public async Task Recheck()
        {
            // Check if the library ID has changed
            if (libraryStore.LibraryId == currentLibraryId) return;
            if (libraryStore.FolderPath == null) return;
            
            // if the library has changed, or the current track was removed, or anything else that could happen on another pane, we gotta take that into account.
            metadataManager = metadataManagerFactory.Create(libraryStore.FolderPath);
            var playlists = await metadataManager.GetPlaylists();
            if (playlists.Count == 0)
            {
                // No playlists exist in the current library
                mediaPlayer.Stop();
                IsPlaying = false;
                return;
            }

            //TEMP
            var firstPlaylist = playlists.Where(x => x.IsAll).First();
            var tracks = await metadataManager.GetPlaylistTracks(firstPlaylist.PlaylistId);
            if (tracks.Count == 0) return;
            playlistNavigator = new PlaylistNavigator(tracks);


        }
    }
}
