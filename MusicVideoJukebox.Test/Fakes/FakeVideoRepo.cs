using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeVideoRepo : IVideoRepo
    {
        public bool TablesCreated = false;
        public bool RanTableCreate = false;
        public List<VideoMetadata> MetadataEntries = [];
        public List<VideoMetadata> UpdatedEntries = [];
        public List<Tuple<int, int>> AppendedToPlaylist = [];
        public List<VideoAnalysisEntry> AnalysisEntries = [];
        public List<Playlist> Playlists = [];
        public List<PlaylistTrack> PlaylistTracks = [];
        public PlaylistStatus CurrentActivePlaylistStatus = new();
        public int? LastPlaylistQueried = null;

        public Task UpdateCurrentSongOrder(int songOrder)
        {
            CurrentActivePlaylistStatus.SongOrder = songOrder;
            return Task.CompletedTask;
        }

        public Task UpdateActivePlaylist(int playlistId)
        {
            CurrentActivePlaylistStatus.PlaylistId = playlistId;
            return Task.CompletedTask;
        }

        public Task UpdatePlayStatus(int playlistId, int songOrder)
        {
            CurrentActivePlaylistStatus.SongOrder = songOrder;
            return Task.CompletedTask;
        }

        public IEnumerable<char>? FolderPath { get; internal set; }
        public Task AddBasicInfos(List<BasicInfo> basicInfos)
        {
            MetadataEntries.AddRange(basicInfos.Select(x => new VideoMetadata { Artist = x.Artist, Filename = x.Filename, Title = x.Title }));
            return Task.CompletedTask;
        }

        public Task<int> AppendSongToPlaylist(int playlistId, int videoId)
        {
            AppendedToPlaylist.Add(new Tuple<int, int>(playlistId, videoId ));
            return Task.FromResult(1);
        }

        public async Task InitializeDatabase()
        {
            await Task.CompletedTask;
            RanTableCreate = true;
            TablesCreated = true;
        }

        public Task DeleteFromPlaylist(int playlistId, int videoId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<VideoMetadata>> GetAllMetadata()
        {
            await Task.CompletedTask;
            return MetadataEntries;
        }

        public async Task<List<VideoAnalysisEntry>> GetAnalysisResults()
        {
            await Task.CompletedTask;
            return AnalysisEntries;
        }

        public Task<List<Playlist>> GetPlaylists()
        {
            return Task.FromResult(Playlists);
        }

        public Task<List<PlaylistTrackForViewmodel>> GetPlaylistTrackForViewmodels(int playlistId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PlaylistTrack>> GetPlaylistTracks(int playlistId)
        {
            LastPlaylistQueried = playlistId;
            await Task.CompletedTask;
            return new List<PlaylistTrack>(PlaylistTracks);
        }

        public Task<int> GetTrackCountForPlaylist(int playlistId)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAnalysisResult(VideoAnalysisEntry entry)
        {
            await Task.CompletedTask;
            AnalysisEntries.Add(entry);
        }

        public Task RemoveMetadata(int videoId)
        {
            MetadataEntries.Remove(MetadataEntries.Where(x => x.VideoId == videoId).First());
            return Task.CompletedTask;
        }

        public Task<int> InsertPlaylist(Playlist playlist)
        {
            Playlists.Add(playlist);
            var id = Playlists.Count;
            playlist.PlaylistId = id;
            return Task.FromResult(id);
        }

        public Task UpdateAnalysisVolume(int videoId, double? lufs)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMetadata(VideoMetadata metadata)
        {
            UpdatedEntries.Add(metadata);
            return Task.CompletedTask;
        }

        public Task UpdatePlaylistDetails(Playlist playlist)
        {
            foreach (var item in Playlists)
            {
                if (item.PlaylistId == playlist.PlaylistId)
                {
                    item.PlaylistName = playlist.PlaylistName;
                    item.Description = playlist.Description;
                    item.ImagePath = playlist.ImagePath;
                    return Task.CompletedTask;
                }
            }
            throw new Exception("Playlist not found");
        }

        public Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order)
        {
            throw new NotImplementedException();
        }

        public Task<PlaylistStatus> GetActivePlaylist()
        {
            return Task.FromResult(CurrentActivePlaylistStatus);
        }

        public Task<bool> IsDatabaseInitialized()
        {
            return Task.FromResult(TablesCreated);
        }

        public Task UpdateAnalysisResult(VideoAnalysisEntry entry)
        {
            throw new NotImplementedException();
        }

        public Task<int> AddTag(string tagName)
        {
            throw new NotImplementedException();
        }

        public Task AddTagToVideo(int videoId, int tagId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTagFromVideo(int videoId, int tagId)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetTagsForVideo(int videoId)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlaylistTrackOrderBatch(List<(int playlistId, int videoId, int order)> updates)
        {
            return Task.CompletedTask;
        }
    }
}