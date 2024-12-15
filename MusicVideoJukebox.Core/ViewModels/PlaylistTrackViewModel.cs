using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistTrackViewModel : BaseViewModel
    {
        private readonly PlaylistTrackForViewmodel playlistTrack;
        public bool IsModified { get; set; } = false;

        public PlaylistTrackViewModel(PlaylistTrackForViewmodel playlistTrack)
        {
            this.playlistTrack = playlistTrack;
        }

        public int PlaylistOrder
        {
            get => playlistTrack.PlayOrder;
            set => SetUnderlyingProperty(playlistTrack.PlayOrder, value, v => { playlistTrack.PlayOrder = v; IsModified = true; });
        }

        public string Name => $"{playlistTrack.Artist} - {playlistTrack.Title}";
        public int VideoId => playlistTrack.VideoId;

        public PlaylistTrackForViewmodel PlaylistTrack => playlistTrack;
    }
}
