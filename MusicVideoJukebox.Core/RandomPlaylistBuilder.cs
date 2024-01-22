using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    public class RandomPlaylistBuilder
    {
        private readonly string dbPath;

        public RandomPlaylistBuilder(string dbPath)
        {
            this.dbPath = dbPath;
        }

        public async Task BuildAsync()
        {
            using var destConnection = new SQLiteConnection($"Data Source={dbPath}");
            destConnection.Open();
            await destConnection.ExecuteAsync(@"
                create table if not exists playlists (
                playlist_id integer primary key autoincrement,
                playlist_name text not null
                )
                ");
            await destConnection.ExecuteAsync(@"
                create table if not exists playlists_videos (
                playlists_videos_id integer primary key autoincrement,
                playlist_id integer not null,
                video_id integer not null,
                play_order integer not null
                )
                ");
            var videoRows = await destConnection.QueryAsync<VideoRow>("SELECT video_id, filename, \"year\", title, album, artist FROM videos");
            var videoRowsShuffled = videoRows.OrderBy(a => Guid.NewGuid()).ToList();
            var trans = await destConnection.BeginTransactionAsync();
            if (trans.Connection == null) { return; }
            var id = await trans.Connection.ExecuteScalarAsync<int>("insert into playlists (playlist_name) values (@PlaylistName) returning playlist_id",
                new PlaylistRow { PlaylistName = "All Songs Alphabetical" });
            var m2mrows = videoRowsShuffled.Select((x, index) => new PlaylistsVideosRow { playlist_id = id, video_id = x.video_id, play_order = index + 1 });
            await trans.Connection.ExecuteAsync("insert into playlists_videos (playlist_id, video_id, play_order) values (@playlist_id, @video_id, @play_order)", m2mrows);
            await trans.CommitAsync();
            destConnection.Close();
        }
    }
}
