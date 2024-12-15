using MusicVideoJukebox.Core;
using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Fakes
{
    internal class FakeVideoRepo : IVideoRepo
    {
        public bool TablesCreated = false;
        public List<VideoMetadata> MetadataEntries = [];
        public List<VideoMetadata> UpdatedEntries = [];
        public List<Tuple<int, int>> AppendedToPlaylist = [];

        public IEnumerable<char>? FolderPath { get; internal set; }
        public Task AddBasicInfos(List<BasicInfo> basicInfos)
        {
            MetadataEntries.AddRange(basicInfos.Select(x => new VideoMetadata { Artist = x.Artist, Filename = x.Filename, Title = x.Title }));
            return Task.CompletedTask;
        }

        public Task AppendSongToPlaylist(int playlistId, int videoId)
        {
            AppendedToPlaylist.Add(new Tuple<int, int>(playlistId, videoId ));
            return Task.CompletedTask;
        }

        public async Task CreateTables()
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

        public Task<List<Playlist>> GetPlaylists()
        {
            throw new NotImplementedException();
        }

        public Task<List<PlaylistTrackForViewmodel>> GetPlaylistTrackForViewmodels(int playlistId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTrackCountForPlaylist(int playlistId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveMetadata(int videoId)
        {
            MetadataEntries.Remove(MetadataEntries.Where(x => x.VideoId == videoId).First());
            return Task.CompletedTask;
        }

        public Task<int> SavePlaylist(Playlist playlist)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMetadata(VideoMetadata metadata)
        {
            UpdatedEntries.Add(metadata);
            return Task.CompletedTask;
        }

        public Task UpdatePlaylistName(int id, string name)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlaylistTrackOrder(int playlistId, int videoId, int order)
        {
            throw new NotImplementedException();
        }
    }
}