using Dapper;
using System.Data.SQLite;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MusicVideoJukebox.Core.Metadata
{
    public class ReferenceDataRepo : IReferenceDataRepo
    {
        private string connectionString;

        public ReferenceDataRepo(string dbPath)
        {
            connectionString = $"Data Source={dbPath};Pooling=False;";
        }

        public async Task<List<SearchResult>> SearchReferenceDb(string artist, string title)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using var conn = new SQLiteConnection(connectionString);
            var normalizedArtist = NormalizeString(artist);
            var normalizedTitle = NormalizeString(title);
            var query = $"%{normalizedArtist}%{normalizedTitle}%";
            var result = await conn.QueryAsync<SearchResult>(@"
                select track_name title, artist_name artist, album_title, release_year
                from tracks t
                join albums al on t.album_id = al.album_id
                join artists ar on t.artist_id = ar.artist_id
                where artist_title like @query
                limit 50
            ", new { query });
            return result.ToList();
        }

        private string NormalizeString(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            var result = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            result = Regex.Replace(result, @"\p{P}(?=\p{L})", ""); // Remove special characters abutting letters
            result = Regex.Replace(result, @"\p{P}", " "); // Replace remaining special characters with space
            result = Regex.Replace(result, @"\s+", " "); // Ensure only one space between words
            result = result.Trim(); // Trim leading and trailing spaces
            return result;
        }

        //public async Task<MetadataGetResult> TryGetExactMatch(string artist, string track)
        //{
        //    Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        //    using var conn = new SQLiteConnection(connectionString);
        //    var maybeMatch = await conn.QueryFirstOrDefaultAsync<FetchedMetadata>(
        //    @"SELECT track_name, artist_name, album_title, first_release_date_year
        //      FROM albums A
        //      JOIN artists B ON B.artist_id = A.artist_id
        //      JOIN tracks C ON C.album_id = A.album_id
        //      WHERE track_name LIKE @track
        //      AND (artist_name LIKE @artist OR artist_name LIKE 'the ' || @artist)
        //      LIMIT 1",
        //    new { track, artist });

        //    if (maybeMatch != null)
        //    {
        //        return new MetadataGetResult { FetchedMetadata = maybeMatch, Success = true };
        //    }
        //    return new MetadataGetResult { Success = false };
        //}

        //const int SEARCH_LENGTH = 3;

        //public async Task <List<FetchedMetadata>> GetCandidates(string artist, string track, int search_length = SEARCH_LENGTH)
        //{
        //    // Remove "The " prefix from the search terms if it exists
        //    string processedArtist = artist.ToLower().StartsWith("the ", StringComparison.OrdinalIgnoreCase) ? artist[4..] : artist;
        //    string processedTrack = track.ToLower().StartsWith("the ", StringComparison.OrdinalIgnoreCase) ? track[4..] : track;

        //    if (processedArtist.Length > search_length)
        //    {
        //        processedArtist = processedArtist[..search_length];
        //    }
        //    if (processedTrack.Length > search_length)
        //    {
        //        processedTrack = processedTrack[..search_length];
        //    }

        //    Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        //    using var conn = new SQLiteConnection(connectionString);
        //    var maybeMatch = await conn.QueryAsync<FetchedMetadata>(
        //    @"SELECT track_name, artist_name, album_title, first_release_date_year
        //      FROM albums A
        //      JOIN artists B ON B.artist_id = A.artist_id
        //      JOIN tracks C ON C.album_id = A.album_id
        //      WHERE (track_name LIKE @track OR track_name LIKE 'the ' || @track)
        //      AND (artist_name LIKE @artist OR artist_name LIKE 'the ' || @artist)
        //      LIMIT 101",
        //    new { track = processedTrack + "%", artist = processedArtist + "%"});
        //    return maybeMatch.ToList();
        //}
    }
}
