namespace MusicVideoJukebox.Core.Metadata
{
    public class PlaylistNavigator
    {
        private readonly IMetadataManager metadataManager;
        List<PlaylistTrack> CurrentTracks = null!;
        public int CurrentPlaylistId { get; private set; }

        public PlaylistNavigator(IMetadataManager metadataManager)
        {
            this.metadataManager = metadataManager;
        }

        private int currentIndex;

        public PlaylistTrack CurrentTrack => CurrentTracks[currentIndex];
        public int SongOrder => currentIndex + 1;

        public PlaylistTrack Next()
        {
            currentIndex = (currentIndex + 1) % CurrentTracks.Count;
            _ = metadataManager.VideoRepo.UpdatePlayStatus(CurrentPlaylistId, SongOrder);
            return CurrentTrack;
        }

        public PlaylistTrack Previous()
        {
            currentIndex = (currentIndex - 1 + CurrentTracks.Count) % CurrentTracks.Count;
            _ = metadataManager.VideoRepo.UpdatePlayStatus(CurrentPlaylistId, SongOrder);
            return CurrentTrack;
        }

        public async Task<PlaylistTrack?> Resume()
        {
            var activePlaylistStatus = await metadataManager.VideoRepo.GetActivePlaylist(); // this is guaranteed to always be set.
            CurrentPlaylistId = activePlaylistStatus.PlaylistId;
            CurrentTracks = await metadataManager.VideoRepo.GetPlaylistTracks(activePlaylistStatus.PlaylistId);
            if (CurrentTracks.Count == 0) return null; // We cannot proceed until there are songs.
            if (activePlaylistStatus.SongOrder == null)
            {
                currentIndex = 0;
                await metadataManager.VideoRepo.UpdatePlayStatus(CurrentPlaylistId, 1);
            }
            else
            {
                currentIndex = (int)activePlaylistStatus.SongOrder - 1;
            }
            return CurrentTrack;
        }

        public async Task<RestoreStateResult> CheckPlaylistState()
        {
            var previousTrack = CurrentTrack;

            // Refresh the tracks
            CurrentTracks = await metadataManager.VideoRepo.GetPlaylistTracks(CurrentPlaylistId);

            // What if all tracks were removed?
            if (CurrentTracks.Count == 0)
            {
                return new RestoreStateResult { NeedsToChangeTrack = true, NewPlaylistTrack = null };
            }

            // How should we handle it if we're at track 5 and a new track is added above us?
            // Do we continue playing this video?
            // I vote yes as long as this video is in the playlist.
            // Note that this will fail if the video is in the playlist multiple times.
            var maybeSameSongInNewOrder = CurrentTracks.Where(x => x.VideoId == previousTrack.VideoId).FirstOrDefault();
            if (maybeSameSongInNewOrder == null)
            {
                // This song is removed from the playlist entirely

                var maybeSamePlaceInPlaylist = CurrentTracks.Where(x => x.PlayOrder == previousTrack.PlayOrder).FirstOrDefault();
                if (maybeSamePlaceInPlaylist == null)
                {
                    // If it got shorter, let's just go back to the start.
                    // Update the currentIndex
                    // Update the database
                    currentIndex = 0;
                    await metadataManager.VideoRepo.UpdatePlayStatus(CurrentPlaylistId, 1);
                }
                else
                {
                    // If the song order still exists (the playlist hasn't gotten shorter), then go to the song at the same play order
                    var index = CurrentTracks.IndexOf(maybeSamePlaceInPlaylist);
                    currentIndex = index;
                    // Play order stays the same, so we can leave the database alone
                }

                return new RestoreStateResult { NeedsToChangeTrack = true, NewPlaylistTrack = CurrentTrack };
            }
            else
            {
                // The song is still in the playlist.
                currentIndex = CurrentTracks.IndexOf(maybeSameSongInNewOrder);
                await metadataManager.VideoRepo.UpdatePlayStatus(CurrentPlaylistId, maybeSameSongInNewOrder.PlayOrder);
                return new RestoreStateResult { NeedsToChangeTrack = false };
            }
        }
    }

    public class RestoreStateResult
    {
        public bool NeedsToChangeTrack { get; internal set; }
        public PlaylistTrack? NewPlaylistTrack { get; internal set; }
    }
}
