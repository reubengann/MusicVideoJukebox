
using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Core.Navigation;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistSelectViewModel : AsyncInitializeableViewModel
    {
        private IMetadataManager? metadataManager;
        private readonly LibraryStore libraryStore;
        private readonly IMetadataManagerFactory metadataManagerFactory;
        private readonly INavigationService navigationService;

        public ICommand SelectPlaylistCommand { get; }

        public ObservableCollection<PlaylistViewModel> Items { get; } = [];

        public PlaylistSelectViewModel(LibraryStore libraryStore, IMetadataManagerFactory metadataManagerFactory, INavigationService navigationService)
        {
            SelectPlaylistCommand = new DelegateCommand<PlaylistViewModel>(SelectPlaylist);
            this.libraryStore = libraryStore;
            this.metadataManagerFactory = metadataManagerFactory;
            this.navigationService = navigationService;
        }

        private async void SelectPlaylist(PlaylistViewModel model)
        {
            await libraryStore.SetPlaylist(model.Playlist.PlaylistId);
            await navigationService.NavigateToNothing();
        }

        public override async Task Initialize()
        {
            if (libraryStore.CurrentState.LibraryPath == null) return;
            metadataManager = metadataManagerFactory.Create(libraryStore.CurrentState.LibraryPath);
            var playlists = await metadataManager.GetPlaylists();
            foreach (var playlist in playlists)
            {
                Items.Add(new PlaylistViewModel(playlist));
            }
        }
    }
}
