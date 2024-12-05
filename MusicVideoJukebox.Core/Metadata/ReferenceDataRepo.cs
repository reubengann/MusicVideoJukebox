using Dapper;
using System.Data.SQLite;

namespace MusicVideoJukebox.Core.Metadata
{
    public class ReferenceDataRepo : IReferenceDataRepo
    {
        private string connectionString;

        public ReferenceDataRepo(string dbPath)
        {
            connectionString = $"Data Source={dbPath};Pooling=False;";
        }

        public async Task<MetadataGetResult> TryGetExactMatch(string artist, string track)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using var conn = new SQLiteConnection(connectionString);
            var maybeMatch = await conn.QueryFirstOrDefaultAsync<FetchedMetadata>(
            @"SELECT track_name, artist_name, album_title, first_release_date_year
              FROM albums A
              JOIN artists B ON B.artist_id = A.artist_id
              JOIN tracks C ON C.album_id = A.album_id
              WHERE track_name LIKE @track COLLATE NOCASE AND artist_name LIKE @artist COLLATE NOCASE",
            new { track, artist });

            if (maybeMatch != null)
            {
                return new MetadataGetResult { FetchedMetadata = maybeMatch, Success = true };
            }
            return new MetadataGetResult { Success = false };
        }
    }
}
