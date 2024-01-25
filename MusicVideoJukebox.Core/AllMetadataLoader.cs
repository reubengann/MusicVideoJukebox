using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    public class VideoInfoAndOrder
    {
        public int PlayOrder { get; set; }
        public VideoInfoWithId Info { get; set; } = null!;
    }

    public class MetadataUpdater : IDisposable
    {
        readonly SQLiteConnection conn;

        public MetadataUpdater(string folder)
        {
            var databaseFile = Path.Combine(folder, "meta.db");
            conn = new SQLiteConnection($"Data Source={databaseFile}");
        }

        public async Task<Playlist> AddNewPlaylist(string playlistName)
        {
            var id = await conn.ExecuteScalarAsync<int>("insert into playlists (playlist_name) values (@PlaylistName) returning playlist_id", new { PlaylistName = playlistName });
            return new Playlist { PlaylistId = id, PlaylistName = playlistName };
        }

        public void Dispose()
        {
            conn.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task UpdatePlaylistName(int playlistId, string newName)
        {
            await conn.ExecuteAsync("update playlists set playlist_name = @NewName where playlist_id = @PlaylistId", new { NewName = newName, PlaylistId = playlistId });
        }

        public async Task UpdateTracksInPlaylist(int playlistId, IEnumerable<int> added, IEnumerable<int> removed)
        {
            await conn.OpenAsync();
            var existing = (await conn.QueryAsync<PlaylistsVideosRow>("select playlists_videos_id, playlist_id, video_id, play_order from playlists_videos where playlist_id = @PlaylistId", new { PlaylistId = playlistId })).ToList();
            var trans = await conn.BeginTransactionAsync();
            if (trans.Connection == null) return;
            await trans.Connection.ExecuteAsync("delete from playlists_videos where playlist_id = @PlaylistId", new { PlaylistId = playlistId });

            foreach (int id in removed)
            {
                existing.RemoveAll(x => x.video_id == id);
            }
            foreach (int id in added)
            {
                existing.Add(new PlaylistsVideosRow { playlist_id = playlistId, video_id = id, play_order = 0 });
            }
            int i = 1;
            foreach (var e in existing)
            {
                e.play_order = i;
                i++;
            }
            await conn.ExecuteAsync("insert into playlists_videos (playlist_id, video_id, play_order) values (@playlist_id, @video_id, @play_order)", existing);
            await trans.CommitAsync();
        }
    }
}
