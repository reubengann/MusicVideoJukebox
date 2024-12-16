using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class VideoInfoViewModel : BaseViewModel
    {
        private readonly PlaylistTrack playlistTrack;

        public VideoInfoViewModel(PlaylistTrack playlistTrack)
        {
            this.playlistTrack = playlistTrack;
        }

        public string Artist => playlistTrack.Artist;
        public string Title => $"\"{playlistTrack.Title}\"";
        public string Album => playlistTrack.Album ?? "";
        public string Year => playlistTrack.ReleaseYear?.ToString() ?? "";
    }
}
