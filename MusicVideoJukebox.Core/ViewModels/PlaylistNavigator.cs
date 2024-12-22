using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Core.ViewModels
{
    public class PlaylistNavigator
    {
        private readonly List<PlaylistTrack> tracks;
        private int currentIndex;

        public PlaylistNavigator(List<PlaylistTrack> tracks)
        {
            this.tracks = tracks ?? throw new ArgumentNullException(nameof(tracks));
            currentIndex = 0; // Start at the first track by default
        }

        public PlaylistTrack CurrentTrack => tracks[currentIndex];

        public PlaylistTrack Next()
        {
            currentIndex = (currentIndex + 1) % tracks.Count;
            return CurrentTrack;
        }

        public PlaylistTrack PlayFirst()
        {
            currentIndex = 0;
            return CurrentTrack;
        }

        public PlaylistTrack Previous()
        {
            currentIndex = (currentIndex - 1 + tracks.Count) % tracks.Count;
            return CurrentTrack;
        }

        public void SetCurrentTrack(int videoId)
        {
            var trackIndex = tracks.FindIndex(track => track.VideoId == videoId);
            if (trackIndex >= 0)
            {
                currentIndex = trackIndex;
            }
            else
            {
                currentIndex = 0;
            }
        }
    }
}
