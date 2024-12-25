
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
                Items.Add(new PlaylistViewModel(playlist, libraryStore));
            }
        }
    }

    public class PlaylistSelectDesignTimeViewModel
    {
        public ObservableCollection<PlaylistDesignTimeViewModel> Items { get; } = [];

        public PlaylistSelectDesignTimeViewModel()
        {
            Items.Add(new PlaylistDesignTimeViewModel { Name = "All songs" });
            Items.Add(new PlaylistDesignTimeViewModel { Name = "Foo", ImagePath = @"C:\Users\Reuben\Videos\Test Folder\images\4236d502-1fe3-490d-833c-ebc9cd03f636.png" });
        }
    }

    public class PlaylistDesignTimeViewModel
    {
        private string imagePath = "";

        required public string Name { get; set; }
        public string ImagePath { get => imagePath ?? "pack://application:,,,/Images/library_music.png"; set => imagePath = value; }
        public string Description { get; set; } = "(no description)";
    }
}
