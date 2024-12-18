
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistPlayViewModel : AsyncInitializeableViewModel
    {
        private IMetadataManager? metadataManager;
        private readonly LibraryStore libraryStore;
        private readonly IMetadataManagerFactory metadataManagerFactory;

        public ObservableCollection<PlaylistViewModel> Items { get; } = [];

        public PlaylistPlayViewModel(LibraryStore libraryStore, IMetadataManagerFactory metadataManagerFactory)
        {
            this.libraryStore = libraryStore;
            this.metadataManagerFactory = metadataManagerFactory;
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
