using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        private readonly Playlist playlist;
        public bool IsModified { get; set; } = false;

        public PlaylistViewModel(Playlist playlist)
        {
            this.playlist = playlist;
        }

        public string Name { get => playlist.PlaylistName; set => SetUnderlyingProperty(playlist.PlaylistName, value, v => { playlist.PlaylistName = v; IsModified = true; }); }
        public int Id { get => playlist.PlaylistId; set => SetUnderlyingProperty(playlist.PlaylistId, value, v => playlist.PlaylistId = v); }
        public Playlist Playlist => playlist;
    }

    public class AvailableTrackViewModel : BaseViewModel
    {
        private readonly VideoMetadata meta;
        public bool IsModified { get; set; } = false;

        public AvailableTrackViewModel(VideoMetadata meta)
        {
            this.meta = meta;
        }

        public string Name => $"{meta.Artist} - {meta.Title}";
        public VideoMetadata Metadata => meta;
    }

    //public class PlaylistTrackViewModel : BaseViewModel
    //{
    //    private readonly PlaylistTrack playlistTrack;
    //    public bool IsModified { get; set; } = false;

    //    public PlaylistTrackViewModel(PlaylistTrack playlistTrack)
    //    {
    //        this.playlistTrack = playlistTrack;
    //    }

    //    public int PlaylistOrder
    //    {
    //        get => playlistTrack.PlaylistOrder;
    //        set => SetUnderlyingProperty(playlistTrack.PlaylistOrder, value, v => { playlistTrack.PlaylistOrder = v; IsModified = true; });
    //    }

    //    public string Name => $"{playlistTrack.Metadata.Artist} - {playlistTrack.Metadata.Title}";
    //    public int VideoId => playlistTrack.Metadata.VideoId;

    //    public PlaylistTrack PlaylistTrack => playlistTrack;
    //}

    public class PlaylistEditViewModel : AsyncInitializeableViewModel
    {
        private readonly LibraryStore libraryStore;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        IMetadataManager metadataManager = null!;
        private PlaylistViewModel? selectedPlaylist;

        public DelegateCommand AddPlaylistCommand { get; }
        public DelegateCommand SavePlaylistCommand { get; }
        public DelegateCommand DeletePlaylistCommand { get; }
        public PlaylistViewModel? SelectedPlaylist 
        { 
            get => selectedPlaylist; 
            set 
            { 
                SetProperty(ref selectedPlaylist, value); 
                RefreshButtons(); 
                OnPropertyChanged(nameof(CanEditTracks));
                //LoadTracksForSelectedPlaylist();
            } 
        }

        //private void LoadTracksForSelectedPlaylist()
        //{
        //    PlaylistTracks.Clear();

        //    if (SelectedPlaylist == null || SelectedPlaylist.Id == -1)
        //    {
        //        return; // Nothing to load for new or null playlists
        //    }
        //}

        public bool CanEditTracks => SelectedPlaylist != null && SelectedPlaylist.Id > 0;

        public ObservableCollection<PlaylistViewModel> Playlists { get; set; } = [];
        public ObservableCollection<AvailableTrackViewModel> AvailableTracks { get; set; } = [];

        void RefreshButtons()
        {
            AddPlaylistCommand.RaiseCanExecuteChanged();
            SavePlaylistCommand.RaiseCanExecuteChanged();
            DeletePlaylistCommand.RaiseCanExecuteChanged();
        }

        public PlaylistEditViewModel(LibraryStore libraryStore, IMetadataManagerFactory metadataManagerFactory)
        {
            this.libraryStore = libraryStore;
            this.metadataManagerFactory = metadataManagerFactory;
            AddPlaylistCommand = new DelegateCommand(AddPlaylist, CanAddPlaylist);
            SavePlaylistCommand = new DelegateCommand(SavePlaylist, CanSavePlaylist);
            DeletePlaylistCommand = new DelegateCommand(DeletePlaylist, CanDeletePlaylist);
        }

        private bool CanDeletePlaylist()
        {
            return SelectedPlaylist != null;
        }

        private void DeletePlaylist()
        {
            throw new NotImplementedException();
        }

        private bool CanSavePlaylist()
        {
            if (SelectedPlaylist == null) return false;
            return SelectedPlaylist.IsModified;
        }

        private async void SavePlaylist()
        {
            if (SelectedPlaylist == null) return;
            if (SelectedPlaylist.Id == -1)
            {
                var id = await metadataManager.SavePlaylist(SelectedPlaylist.Playlist);
                SelectedPlaylist.Id = id;
            }
            else
            {
                await metadataManager.UpdatePlaylist(SelectedPlaylist.Playlist);
            }
            SelectedPlaylist.IsModified = false;
            RefreshButtons();
        }

        private bool CanAddPlaylist()
        {
            if (libraryStore.FolderPath == null) return false;
            if (SelectedPlaylist == null) return true;
            return !SelectedPlaylist.IsModified;
        }

        private void AddPlaylist()
        {
            string baseName = "New Playlist";
            string newName = GenerateUniquePlaylistName(baseName);

            var newPlaylist = new Playlist
            {
                PlaylistId = -1, // Indicating it's a new playlist that hasn't been saved yet
                PlaylistName = newName
            };

            var newPlaylistViewModel = new PlaylistViewModel(newPlaylist)
            {
                IsModified = true
            };

            Playlists.Add(newPlaylistViewModel);
            SelectedPlaylist = newPlaylistViewModel;
            OnPropertyChanged(nameof(SelectedPlaylist));
        }

        public override async Task Initialize()
        {
            if (libraryStore.FolderPath == null) return;
            metadataManager = metadataManagerFactory.Create(libraryStore.FolderPath);
            var playlists = await metadataManager.GetPlaylists();
            foreach (var pl in playlists)
            {
                Playlists.Add(new PlaylistViewModel(pl));
            }
            var avail = await metadataManager.GetAllMetadata();
            foreach (var a in avail)
            {
                AvailableTracks.Add(new AvailableTrackViewModel(a));
            }
        }

        private string GenerateUniquePlaylistName(string baseName)
        {
            var existingNames = Playlists.Select(p => p.Name).ToHashSet();
            if (!existingNames.Contains(baseName))
            {
                return baseName;
            }

            int counter = 2;
            string uniqueName;
            do
            {
                uniqueName = $"{baseName} {counter}";
                counter++;
            } while (existingNames.Contains(uniqueName));

            return uniqueName;
        }
    }
}
