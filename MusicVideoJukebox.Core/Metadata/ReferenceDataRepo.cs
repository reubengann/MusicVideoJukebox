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
              WHERE track_name LIKE @track
              AND (artist_name LIKE @artist OR artist_name LIKE 'the ' || @artist)
              LIMIT 1",
            new { track, artist });

            if (maybeMatch != null)
            {
                return new MetadataGetResult { FetchedMetadata = maybeMatch, Success = true };
            }
            return new MetadataGetResult { Success = false };
        }

        int SEARCH_LENGTH = 3;

        public async Task <List<FetchedMetadata>> GetCandidates(string artist, string track)
        {
            // Remove "The " prefix from the search terms if it exists
            string processedArtist = artist.ToLower().StartsWith("the ", StringComparison.OrdinalIgnoreCase) ? artist[4..] : artist;
            string processedTrack = track.ToLower().StartsWith("the ", StringComparison.OrdinalIgnoreCase) ? track[4..] : track;

            if (processedArtist.Length > SEARCH_LENGTH)
            {
                processedArtist = processedArtist[..SEARCH_LENGTH];
            }
            if (processedTrack.Length > SEARCH_LENGTH)
            {
                processedTrack = processedTrack[..SEARCH_LENGTH];
            }

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using var conn = new SQLiteConnection(connectionString);
            var maybeMatch = await conn.QueryAsync<FetchedMetadata>(
            @"SELECT track_name, artist_name, album_title, first_release_date_year
              FROM albums A
              JOIN artists B ON B.artist_id = A.artist_id
              JOIN tracks C ON C.album_id = A.album_id
              WHERE (track_name LIKE @track OR track_name LIKE 'the ' || @track)
              AND (artist_name LIKE @artist OR artist_name LIKE 'the ' || @artist)
              LIMIT 101",
            new { track = processedTrack + "%", artist = processedArtist + "%"});
            return maybeMatch.ToList();
        }
    }
}
