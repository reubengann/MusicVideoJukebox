using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
    }

    public class PlaylistEditViewModel : AsyncInitializeableViewModel
    {
        private readonly LibraryStore libraryStore;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        IMetadataManager metadataManager = null!;
        private PlaylistViewModel? selectedPlaylist;

        public DelegateCommand AddPlaylistCommand { get; }
        public DelegateCommand SavePlaylistCommand { get; }
        public DelegateCommand DeletePlaylistCommand { get; }
        public PlaylistViewModel? SelectedPlaylist { get => selectedPlaylist; set { SetProperty(ref selectedPlaylist, value); AddPlaylistCommand.RaiseCanExecuteChanged(); } }
        public bool CanEditTracks => SelectedPlaylist != null && SelectedPlaylist.Id > 0;

        public ObservableCollection<PlaylistViewModel> Playlists { get; set; } = [];

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

        private void SavePlaylist()
        {
            throw new NotImplementedException();
        }

        private bool CanAddPlaylist()
        {
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
