using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeMetadataManager : IMetadataManager
    {
        public string folderPath;

        public List<string> CreatedMetadataFolders { get; internal set; } = [];
        public List<VideoMetadata> MetadataEntries { get; internal set; } = [];
        public List<VideoMetadata> MetadataEntriesUpdated { get; internal set; } = [];
        public List<VideoAnalysisEntry> AnalysisEntries = [];
        public int SearchCount = 0;
        public PlaylistStatus CurrentActivePlaylistStatus = new();
        public List<ScoredMetadata> ScoredCandidates = [];

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

        public async Task UpdatePlaylist(Playlist playlist)
        {
            foreach (var item in Playlists)
            {
                if (item.PlaylistId == playlist.PlaylistId)
                {
                    item.PlaylistName = playlist.PlaylistName;
                    item.Description = playlist.Description;
                    item.ImagePath = playlist.ImagePath;
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

        public Task<int> AppendSongToPlaylist(int playlistId, int videoId)
        {
            AddedToPlaylist.Add(new Tuple<int, int> (playlistId, videoId));
            return Task.FromResult(1);
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
            return new List<PlaylistTrack>(PlaylistTracks);
        }

        public async Task<List<ScoredMetadata>> GetScoredCandidates(string artist, string track)
        {
            SearchCount++;
            await Task.CompletedTask;
            return ScoredCandidates;
        }

        public async Task InsertAnalysisResult(VideoAnalysisEntry entry)
        {
            await Task.CompletedTask;
            AnalysisEntries.Add(entry);
        }

        public async Task<List<VideoAnalysisEntry>> GetAnalysisResults()
        {
            await Task.CompletedTask;
            return AnalysisEntries;
        }

        public Task UpdateAnalysisVolume(int videoId, double? lufs)
        {
            var foo = AnalysisEntries.Where(x => x.VideoId == videoId).First();
            foo.LUFS = lufs;
            return Task.CompletedTask;
        }

        public Task<PlaylistStatus> GetActivePlaylist()
        {
            return Task.FromResult(CurrentActivePlaylistStatus);
        }

        public Task UpdateCurrentSongOrder(int songOrder)
        {
            CurrentActivePlaylistStatus.SongOrder = songOrder;
            return Task.CompletedTask;
        }
    }
}