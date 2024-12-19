
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistPlayViewModel : AsyncInitializeableViewModel
    {
        private IMetadataManager? metadataManager;
        private readonly LibraryStore libraryStore;
        private readonly IMetadataManagerFactory metadataManagerFactory;

        public ICommand SelectPlaylistCommand { get; }

        public ObservableCollection<PlaylistViewModel> Items { get; } = [];

        public PlaylistPlayViewModel(LibraryStore libraryStore, IMetadataManagerFactory metadataManagerFactory)
        {
            SelectPlaylistCommand = new DelegateCommand<PlaylistViewModel>(SelectPlaylist);
            this.libraryStore = libraryStore;
            this.metadataManagerFactory = metadataManagerFactory;
        }

        private void SelectPlaylist(PlaylistViewModel model)
        {
        }

        public override async Task Initialize()
        {
            if (libraryStore.FolderPath == null) return;
            metadataManager = metadataManagerFactory.Create(libraryStore.FolderPath);
            var playlists = await metadataManager.GetPlaylists();
            foreach (var playlist in playlists)
            {
                Items.Add(new PlaylistViewModel(playlist));
            }
        }
    }
}
