using MusicVideoJukebox.Core.Libraries;
using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        private readonly Playlist playlist;
        private readonly LibraryStore libraryStore;

        public PlaylistViewModel(Playlist playlist, LibraryStore libraryStore)
        {
            this.playlist = playlist;
            this.libraryStore = libraryStore;
        }

        public string Name { get => playlist.PlaylistName; set => SetUnderlyingProperty(playlist.PlaylistName, value, v => { playlist.PlaylistName = v; }); }
        public int Id { get => playlist.PlaylistId; set => SetUnderlyingProperty(playlist.PlaylistId, value, v => playlist.PlaylistId = v); }
        public string ImagePath => GetPath();
        public string Description => playlist.Description ?? "(no description)";

        public bool IsAll => playlist.IsAll;
        public Playlist Playlist => playlist;

        string GetPath()
        {
            if (libraryStore.CurrentState == null
                || libraryStore.CurrentState.LibraryPath == null
                || playlist.ImagePath == null) return "/Images/image_off.png";
            return Path.Combine(libraryStore.CurrentState.LibraryPath, playlist.ImagePath);
        }
    }
}
