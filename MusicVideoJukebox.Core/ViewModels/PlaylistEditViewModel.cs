
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using System.Collections.ObjectModel;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistEditViewModel : AsyncInitializeableViewModel
    {
        private readonly LibraryStore libraryStore;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        IMetadataManager metadataManager = null!;

        public ObservableCollection<Playlist> Playlists { get; set; } = [];

        public PlaylistEditViewModel(LibraryStore libraryStore, IMetadataManagerFactory metadataManagerFactory)
        {
            this.libraryStore = libraryStore;
            this.metadataManagerFactory = metadataManagerFactory;
        }

        public override async Task Initialize()
        {
            if (libraryStore.FolderPath == null) return;
            metadataManager = metadataManagerFactory.Create(libraryStore.FolderPath);
            var playlists = await metadataManager.GetPlaylists();
            foreach (var pl in playlists)
            {
                Playlists.Add(pl);
            }
        }
    }
}
