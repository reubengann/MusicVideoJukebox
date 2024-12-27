using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.UserInterface;
using Prism.Commands;
using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistEditViewModel : AsyncInitializeableViewModel
    {
        private readonly LibraryStore libraryStore;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly IDialogService dialogService;
        private readonly IImageScalerService imageScalerService;
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
        public DelegateCommand EditPlaylistDetailsCommand { get; }
        public DelegateCommand DeletePlaylistCommand { get; }
        public DelegateCommand AddTrackToPlaylistCommand { get; }
        public DelegateCommand DeleteTrackFromPlaylistCommand { get; }
        public DelegateCommand ShufflePlaylistCommand { get; }
        public PlaylistViewModel? SelectedPlaylist
        {
            get => selectedPlaylist;
            set
            {
                SetProperty(ref selectedPlaylist, value);
                OnPropertyChanged(nameof(CanEditTracks));

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

        public PlaylistEditViewModel(LibraryStore libraryStore, IMetadataManagerFactory metadataManagerFactory, IDialogService dialogService, IImageScalerService imageScalerService)
        {
            this.libraryStore = libraryStore;
            this.metadataManagerFactory = metadataManagerFactory;
            this.dialogService = dialogService;
            this.imageScalerService = imageScalerService;
            AddPlaylistCommand = new DelegateCommand(AddPlaylist, CanAddPlaylist);
            EditPlaylistDetailsCommand = new DelegateCommand(LaunchPlaylistDetailsEditor, CanLaunchPlaylistDetailsEditor);
            DeletePlaylistCommand = new DelegateCommand(DeletePlaylist, CanDeletePlaylist);
            AddTrackToPlaylistCommand = new DelegateCommand(AddTracksToPlaylist, CanAddTrackToPlaylist);
            DeleteTrackFromPlaylistCommand = new DelegateCommand(DeleteTrackFromPlaylist, CanDeleteTrackFromPlaylist);
            ShufflePlaylistCommand = new DelegateCommand(ShufflePlaylist);
        }

        public bool ErrorOccurred { get; private set; }

        private async void LaunchPlaylistDetailsEditor()
        {
            if (SelectedPlaylist == null) return;
            try
            {
                var vm = new PlaylistDetailsEditDialogViewModel(SelectedPlaylist.Playlist, dialogService, imageScalerService, libraryStore);
                dialogService.ShowEditPlaylistDetailsDialog(vm);
                if (!vm.Accepted) return;
                await metadataManager.UpdatePlaylist(vm.NewPlaylist);
                SelectedPlaylist.Playlist.Description = vm.NewPlaylist.Description;
                SelectedPlaylist.Playlist.ImagePath = vm.NewPlaylist.ImagePath;
                SelectedPlaylist.Name = vm.NewPlaylist.PlaylistName;
            }
            catch
            {
                ErrorOccurred = true;
            }
        }

        private async void AddPlaylist()
        {
            string newName = GenerateUniquePlaylistName("New Playlist");

            var newPlaylist = new Playlist
            {
                PlaylistId = -1, // Indicating it's a new playlist that hasn't been saved yet
                PlaylistName = newName
            };

            try
            {
                var vm = new PlaylistDetailsEditDialogViewModel(newPlaylist, dialogService, imageScalerService, libraryStore);
                dialogService.ShowEditPlaylistDetailsDialog(vm);
                if (!vm.Accepted) return;
                var newId = await metadataManager.SavePlaylist(vm.NewPlaylist);
                newPlaylist.PlaylistId = newId;
            }
            catch
            {
                ErrorOccurred = true;
                return;
            }

            var newPlaylistViewModel = new PlaylistViewModel(newPlaylist, libraryStore);

            Playlists.Add(newPlaylistViewModel);
            SelectedPlaylist = newPlaylistViewModel;
            OnPropertyChanged(nameof(SelectedPlaylist));
            OnPropertyChanged(nameof(IsPlaylistMutable));
            RefreshButtons();
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
            EditPlaylistDetailsCommand.RaiseCanExecuteChanged();
            DeletePlaylistCommand.RaiseCanExecuteChanged();
            AddTrackToPlaylistCommand.RaiseCanExecuteChanged();
            DeleteTrackFromPlaylistCommand.RaiseCanExecuteChanged();
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
            foreach (var track in selectedTracks)
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

        private bool CanLaunchPlaylistDetailsEditor()
        {
            if (SelectedPlaylist == null) return false;
            return IsPlaylistMutable;
        }

        private bool CanAddPlaylist()
        {
            return libraryStore.CurrentState.LibraryPath != null;
        }

        public override async Task Initialize()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;
            metadataManager = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
            var playlists = await metadataManager.GetPlaylists();
            foreach (var pl in playlists)
            {
                Playlists.Add(new PlaylistViewModel(pl, libraryStore));
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
