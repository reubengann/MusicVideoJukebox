using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistEditViewModel : AsyncInitializeableViewModel
    {
        private readonly LibraryStore libraryStore;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        IMetadataManager metadataManager = null!;
        private PlaylistViewModel? selectedPlaylist;
        private ObservableCollection<PlaylistTrackViewModel> playlistTracks = [];
        public ObservableCollection<PlaylistTrackViewModel> PlaylistTracks
        {
            get => playlistTracks;
            set
            {
                playlistTracks = value;
                OnPropertyChanged(nameof(PlaylistTracks));
            }
        }
        public DelegateCommand AddPlaylistCommand { get; }
        public DelegateCommand SavePlaylistCommand { get; }
        public DelegateCommand DeletePlaylistCommand { get; }
        public DelegateCommand AddTrackToPlaylistCommand { get; }
        public DelegateCommand DeleteTrackFromPlaylistCommand { get; }
        public DelegateCommand ShufflePlaylistCommand { get; }
        public PlaylistViewModel? SelectedPlaylist
        {
            get => selectedPlaylist;
            set
            {

                if (selectedPlaylist != null)
                {
                    selectedPlaylist.PropertyChanged -= SelectedPlaylist_PropertyChanged;
                }
                
                SetProperty(ref selectedPlaylist, value);
                OnPropertyChanged(nameof(CanEditTracks));

                if (SelectedPlaylist != null)
                {
                    SelectedPlaylist.PropertyChanged += SelectedPlaylist_PropertyChanged;
                }

                if (value != null && value.Id > 0)
                {
                    _ = LoadTracksForSelectedPlaylist(); // Fire-and-forget
                }
                else
                {
                    PlaylistTracks.Clear();
                }
                RefreshButtons();
                OnPropertyChanged(nameof(IsPlaylistMutable));
            }
        }

        private void SelectedPlaylist_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlaylistViewModel.Name))
            {
                SavePlaylistCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task LoadTracksForSelectedPlaylist()
        {
            PlaylistTracks.Clear();
            if (SelectedPlaylist == null || SelectedPlaylist.Id == -1)
            {
                return; // Nothing to load for new or null playlists
            }
            var tracks = await metadataManager.GetPlaylistTracksForViewmodel(SelectedPlaylist.Id);
            PlaylistTracks = new ObservableCollection<PlaylistTrackViewModel>(
            tracks.OrderBy(x => x.PlayOrder).Select(track => new PlaylistTrackViewModel(track))
    );
        }

        public bool IsPlaylistMutable
        { 
            get 
            {
                if (SelectedPlaylist == null) return false;
                if (SelectedPlaylist.IsAll) return false;
                return true;
            } 
        }

        public bool CanEditTracks => SelectedPlaylist != null && SelectedPlaylist.Id > 0;

        public ObservableCollection<PlaylistViewModel> Playlists { get; set; } = [];
        public ObservableCollection<AvailableTrackViewModel> AvailableTracks { get; set; } = [];
        public ObservableCollection<AvailableTrackViewModel> FilteredAvailableTracks { get; set; } = [];

        private string availableTracksFilter = string.Empty;
        public string AvailableTracksFilter
        {
            get => availableTracksFilter;
            set
            {
                if (SetProperty(ref availableTracksFilter, value))
                {
                    ApplyAvailableTracksFilter();
                }
            }
        }

        private void ApplyAvailableTracksFilter()
        {
            if (string.IsNullOrWhiteSpace(AvailableTracksFilter))
            {
                // No filter applied; show all tracks
                FilteredAvailableTracks.Clear();
                foreach (var track in AvailableTracks)
                {
                    FilteredAvailableTracks.Add(track);
                }
            }
            else
            {
                // Apply filter (case-insensitive matching against title or artist)
                var filtered = AvailableTracks
                    .Where(track => track.Metadata.Artist.Contains(AvailableTracksFilter, StringComparison.OrdinalIgnoreCase) ||
                                    track.Metadata.Title.Contains(AvailableTracksFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Update the filtered collection
                FilteredAvailableTracks.Clear();
                foreach (var track in filtered)
                {
                    FilteredAvailableTracks.Add(track);
                }
            }
        }

        void RefreshButtons()
        {
            AddPlaylistCommand.RaiseCanExecuteChanged();
            SavePlaylistCommand.RaiseCanExecuteChanged();
            DeletePlaylistCommand.RaiseCanExecuteChanged();
            AddTrackToPlaylistCommand.RaiseCanExecuteChanged();
            DeleteTrackFromPlaylistCommand.RaiseCanExecuteChanged();
        }

        public PlaylistEditViewModel(LibraryStore libraryStore, IMetadataManagerFactory metadataManagerFactory)
        {
            this.libraryStore = libraryStore;
            this.metadataManagerFactory = metadataManagerFactory;
            AddPlaylistCommand = new DelegateCommand(AddPlaylist, CanAddPlaylist);
            SavePlaylistCommand = new DelegateCommand(SavePlaylist, CanSavePlaylist);
            DeletePlaylistCommand = new DelegateCommand(DeletePlaylist, CanDeletePlaylist);
            AddTrackToPlaylistCommand = new DelegateCommand(AddTracksToPlaylist, CanAddTrackToPlaylist);
            DeleteTrackFromPlaylistCommand = new DelegateCommand(DeleteTrackFromPlaylist, CanDeleteTrackFromPlaylist);
            ShufflePlaylistCommand = new DelegateCommand(ShufflePlaylist, CanShufflePlaylist);
        }

        private bool CanShufflePlaylist()
        {
            return true;
        }

        private async void ShufflePlaylist()
        {
            if (SelectedPlaylist == null) return;
            var shuffled = await metadataManager.ShuffleTracks(SelectedPlaylist.Id);
            PlaylistTracks = new ObservableCollection<PlaylistTrackViewModel>(
            shuffled.OrderBy(x => x.PlayOrder).Select(track => new PlaylistTrackViewModel(track))
            );
        }

        private bool CanDeleteTrackFromPlaylist()
        {
            if (SelectedPlaylist == null || SelectedPlaylist.IsAll) return false;
            return true;
        }

        private void DeleteTrackFromPlaylist()
        {
            
        }

        private bool CanAddTrackToPlaylist()
        {
            if (SelectedPlaylist == null || SelectedPlaylist.IsAll) return false;
            return true;
        }

        private async void AddTracksToPlaylist()
        {
            if (SelectedPlaylist == null) return;
            var selectedTracks = FilteredAvailableTracks.Where(x => x.IsSelected);
            int count = PlaylistTracks.Count;
            foreach(var track in selectedTracks)
            {
                count++;
                var playlistVideoId = await metadataManager.AppendSongToPlaylist(SelectedPlaylist.Playlist.PlaylistId, track.Metadata.VideoId);
                PlaylistTracks.Add(new PlaylistTrackViewModel(new PlaylistTrackForViewmodel { Artist = track.Artist, Title = track.Title, PlaylistId = SelectedPlaylist.Id, PlaylistVideoId = playlistVideoId, PlayOrder = count, VideoId = track.Metadata.VideoId }));
            }
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
                await metadataManager.UpdatePlaylistName(SelectedPlaylist.Id, SelectedPlaylist.Name);
            }
            SelectedPlaylist.IsModified = false;
            RefreshButtons();
        }

        private bool CanAddPlaylist()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return false;
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
            OnPropertyChanged(nameof(IsPlaylistMutable));
            RefreshButtons();
        }

        public override async Task Initialize()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;
            metadataManager = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
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
            ApplyAvailableTracksFilter();
            // there should be at least one playlist, but you never know.
            if (Playlists.Count == 0) return;
            SelectedPlaylist = Playlists[0];
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
