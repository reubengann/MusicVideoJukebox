using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core
{
    public static class BackfillVideoDatabaseBuilder
    {
        public static async Task BuildAsync(string libraryDbFile, string destinationFile)
        {
            using (var refConnection = new SQLiteConnection($"Data Source={libraryDbFile}"))
            using (var destConnection = new SQLiteConnection($"Data Source={destinationFile}"))
            {
                refConnection.Open();
                destConnection.Open();
                await destConnection.ExecuteAsync(@"
create table if not exists videos (
video_id integer primary key autoincrement,
filename text NOT NULL,
""year"" integer NULL,
title text NOT NULL, 
album text NULL, 
artist text NOT NULL
)
");
                var fileNames = Directory.EnumerateFiles("E:\\Videos\\Music Videos\\On Media Center").Where(x => x.EndsWith(".mp4")).ToHashSet();
                var existingRows = await destConnection.QueryAsync<VideoRow>("SELECT video_id, filename, \"year\", title, album, artist FROM videos");
                var filesInDb = existingRows.Select(x => Path.Combine("E:\\Videos\\Music Videos\\On Media Center", x.filename)).ToHashSet();
                var toRemove = filesInDb.Except(fileNames);
                var toAdd = fileNames.Except(filesInDb);
                // TODO: Remove items.
                var agent = new LibraryFillInMetadataProvider(refConnection);
                foreach (var row in toAdd)
                {
                    var (artist, title) = FileNameHelpers.ParseFileNameIntoArtistTitle(row);
                    var metadata = await agent.GetMetadata(artist, title);
                    var vr = new VideoRow { artist = artist, title = title, album = metadata.Album, filename = Path.GetFileName(row), year = metadata.Year };
                    await destConnection.ExecuteAsync("INSERT INTO videos (filename, \"year\", title, album, artist) values (@filename, @year, @title, @album, @artist)", vr);
                }
            }
        }
    }
}
