using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        private readonly Playlist playlist;
        public PlaylistViewModel(Playlist playlist)
        {
            this.playlist = playlist;
        }

        public string Name { get => playlist.PlaylistName; set => SetUnderlyingProperty(playlist.PlaylistName, value, v => { playlist.PlaylistName = v; }); }
        public int Id { get => playlist.PlaylistId; set => SetUnderlyingProperty(playlist.PlaylistId, value, v => playlist.PlaylistId = v); }
        public bool IsAll => playlist.IsAll;
        public Playlist Playlist => playlist;
    }
}
