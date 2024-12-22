using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataManager : IMetadataManager
    {
        public string folderPath;

        public List<string> CreatedMetadataFolders { get; internal set; } = [];
        public List<VideoMetadata> MetadataEntries { get; internal set; } = [];
        public List<VideoMetadata> MetadataEntriesUpdated { get; internal set; } = [];
        public bool WasShuffled = false;

        public List<Playlist> Playlists = [];
        public List<PlaylistTrack> PlaylistTracks = [];
        public Dictionary<string, GetAlbumYearResult> ReferenceDataToGet = [];
        public List<Tuple<int, int>> AddedToPlaylist = [];
        public bool SayChangesWereMade = false;
        public int? LastPlaylistQueried = null;

        public FakeMetadataManager(string folderPath)
        {
            this.folderPath = folderPath;
        }

        public async Task EnsureCreated()
        {
            await Task.CompletedTask;
            CreatedMetadataFolders.Add(folderPath);
        }

        public async Task<List<VideoMetadata>> GetAllMetadata()
        {
            await Task.CompletedTask;
            return MetadataEntries;
        }

        public Task UpdateVideoMetadata(VideoMetadata entry)
        {
            MetadataEntriesUpdated.Add(entry);
            return Task.CompletedTask;
        }

        public async Task<GetAlbumYearResult> TryGetAlbumYear(string artist, string track)
        {
            await Task.CompletedTask;
            if (ReferenceDataToGet.ContainsKey($"{artist} {track}"))
            {
                return ReferenceDataToGet[$"{artist} {track}"];
            }
            return new GetAlbumYearResult { Success = false };
        }

        public Task<bool> Resync()
        {
            return Task.FromResult(SayChangesWereMade);
        }

        public Task<List<Playlist>> GetPlaylists()
        {
            return Task.FromResult(Playlists);
        }

        public async Task<int> SavePlaylist(Playlist playlist)
        {
            Playlists.Add(playlist);
            var id = Playlists.Count;
            //playlist.PlaylistId = id;
            return id;
        }

        public async Task UpdatePlaylistName(int id, string name)
        {
            foreach (var item in Playlists)
            {
                if (item.PlaylistId == id)
                {
                    item.PlaylistName = name;
                    return;
                }
            }
        }

        public async Task<List<PlaylistTrackForViewmodel>> GetPlaylistTracksForViewmodel(int playlistId)
        {
            await Task.CompletedTask;
            int i = 0;
            return PlaylistTracks.Select(x => new PlaylistTrackForViewmodel { Artist = x.Artist, Title = x.Title, PlayOrder = i++ }).ToList();
        }

        public Task AppendSongToPlaylist(int playlistId, int videoId)
        {
            AddedToPlaylist.Add(new Tuple<int, int> (playlistId, videoId));
            return Task.CompletedTask;
        }

        public Task<List<PlaylistTrackForViewmodel>> ShuffleTracks(int playlistId)
        {
            WasShuffled = true;
            return Task.FromResult(MetadataEntries.Select(x => new PlaylistTrackForViewmodel { Artist = x.Artist, Title = x.Title }).ToList());
        }

        public Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PlaylistTrack>> GetPlaylistTracks(int playlistId)
        {
            LastPlaylistQueried = playlistId;
            await Task.CompletedTask;
            return PlaylistTracks;
        }
    }
}