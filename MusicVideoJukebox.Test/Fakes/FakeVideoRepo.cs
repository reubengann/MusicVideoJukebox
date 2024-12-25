using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeVideoRepo : IVideoRepo
    {
        public bool TablesCreated = false;
        public List<VideoMetadata> MetadataEntries = [];
        public List<VideoMetadata> UpdatedEntries = [];
        public List<Tuple<int, int>> AppendedToPlaylist = [];
        public List<VideoAnalysisEntry> AnalysisEntries = [];

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
            throw new NotImplementedException();
        }

        public Task<List<PlaylistTrackForViewmodel>> GetPlaylistTrackForViewmodels(int playlistId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlaylistTrack>> GetPlaylistTracks(int playlistId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlayStatus(int playlistId, int videoId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateActivePlaylist(int playlistId)
        {
            throw new NotImplementedException();
        }

        public Task<PlaylistStatus> GetActivePlaylist()
        {
            throw new NotImplementedException();
        }
    }
}