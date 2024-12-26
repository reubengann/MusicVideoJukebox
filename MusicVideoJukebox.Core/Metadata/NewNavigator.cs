
using System.Diagnostics;

namespace MusicVideoJukebox.Core.Metadata
{
    public class NewNavigator
    {
        private readonly IMetadataManager metadataManager;
        List<PlaylistTrack> CurrentTracks = null!;

        public NewNavigator(IMetadataManager metadataManager)
        {
            this.metadataManager = metadataManager;
        }

        private int currentIndex;

        public PlaylistTrack CurrentTrack => CurrentTracks[currentIndex];
        public int SongOrder => currentIndex + 1;

        public PlaylistTrack Next()
        {
            currentIndex = (currentIndex + 1) % CurrentTracks.Count;
            _ = metadataManager.UpdateCurrentSongOrder(SongOrder);
            return CurrentTrack;
        }

        public PlaylistTrack Previous()
        {
            currentIndex = (currentIndex - 1 + CurrentTracks.Count) % CurrentTracks.Count;
            _ = metadataManager.UpdateCurrentSongOrder(SongOrder);
            return CurrentTrack;
        }

        public async Task<PlaylistTrack?> Resume()
        {
            var activePlaylistStatus = await metadataManager.GetActivePlaylist(); // this is guaranteed to always be set.
            CurrentTracks = await metadataManager.GetPlaylistTracks(activePlaylistStatus.PlaylistId);
            if (CurrentTracks.Count == 0) return null; // We cannot proceed until there are songs.
            if (activePlaylistStatus.SongOrder == null)
                currentIndex = 0;
            else
            {
                currentIndex = (int)activePlaylistStatus.SongOrder - 1;
            }
            return CurrentTrack;
        }
    }
}
