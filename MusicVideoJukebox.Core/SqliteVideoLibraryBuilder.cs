using Dapper;
using MusicVideoJukebox.Core.Metadata;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    //public class SqliteVideoLibraryBuilder : IVideoLibraryBuilder
    //{
    //    public async Task<VideoLibrary> BuildAsync(string folder)
    //    {
    //        var databaseFile = Path.Combine(folder, "meta.db");
    //        using var conn = new SQLiteConnection($"Data Source={databaseFile}");
    //        var videoRows = await conn.QueryAsync<VideoRow>(@"select * from videos");
    //        var fileNames = videoRows.ToDictionary(x => x.video_id, x => Path.Combine(folder, x.filename));

    //        var infoMap = videoRows.ToDictionary(x => x.video_id, x => new VideoInfo { Album = x.album, Artist = x.artist, Title = x.title, Year = x.year });
    //        var playlistItemRows = await conn.QueryAsync<VideoIdPlaylistIdPair>("select playlist_id, video_id from playlists_videos order by playlist_id, play_order");
    //        var playlistIdToSongMap = new Dictionary<int, List<int>>();
    //        foreach (var row in playlistItemRows)
    //        {
    //            if (!playlistIdToSongMap.ContainsKey(row.playlist_id))
    //                playlistIdToSongMap[row.playlist_id] = [];
    //            playlistIdToSongMap[row.playlist_id].Add(row.video_id);
    //        }
    //        var playlistRows = await conn.QueryAsync<PlaylistRow>("select playlist_id, playlist_name from playlists order by playlist_id");
    //        var playlistIdToVideosWithOrderMap = new Dictionary<int, List<VideoInfoAndOrder>>();
    //        foreach (var row in playlistRows)
    //        {
    //            var foo = await conn.QueryAsync<PlaylistOrderRow>(@"select A.*, B.play_order from
    //                videos A
    //                join playlists_videos B
    //                ON A.video_id = B.video_id
    //                join playlists C
    //                ON B.playlist_id = C.playlist_id
    //                where C.playlist_id = @PlaylistId
    //                order by B.play_order", new { PlaylistId = row.playlist_id });
    //            playlistIdToVideosWithOrderMap[row.playlist_id] = foo.Select(x => new VideoInfoAndOrder { Info = new VideoInfoWithId { VideoId = x.video_id, Album = x.album, Artist = x.artist, Title = x.title, Year = x.year }, PlayOrder = x.play_order }).ToList();
    //        }

    //        // if there's no status row, create one
    //        var numStatusRows = await conn.ExecuteScalarAsync<int>("select count(*) from play_status");
    //        if (numStatusRows == 0)
    //        {
    //            var firstPlaylistId = playlistRows.First().playlist_id;
    //            var firstSongId = playlistIdToSongMap[firstPlaylistId].First();
    //            await conn.ExecuteAsync("insert into play_status (playlist_id, song_id) values (@PlaylistId, @SongId)", new { PlaylistId = firstPlaylistId, SongId = firstSongId });
    //        }
    //        var progressPersister = await ProgressPersister.CreateAsync(databaseFile);

    //        return new VideoLibrary(fileNames, infoMap, folder, playlistIdToSongMap, playlistRows.Select(x => new Playlist { PlaylistId = x.playlist_id, PlaylistName = x.playlist_name }).ToList(),
    //            videoRows.Select(x => new VideoInfoWithId { VideoId = x.video_id, Album = x.album, Artist = x.artist, Title = x.title, Year = x.year }).ToList(),
    //            playlistIdToVideosWithOrderMap,
    //            progressPersister
    //            );
    //    }

    //    class PlaylistOrderRow
    //    {
    //        public int video_id { get; set; }
    //        public string filename { get; set; } = null!;
    //        public int? year { get; set; }
    //        public string title { get; set; } = null!;
    //        public string? album { get; set; }
    //        public string artist { get; set; } = null!;
    //        public int play_order { get; set; }

    //    }

    //    class VideoIdPlaylistIdPair
    //    {
    //        public int video_id { get; set; }
    //        public int playlist_id { get; set; }
    //    }
    //}
}
