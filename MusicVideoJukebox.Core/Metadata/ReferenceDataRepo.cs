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

        public async Task<List<SearchResult>> SearchReferenceDb(string queryString)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            using var conn = new SQLiteConnection(connectionString);
            var normalizedArtist = "%" + string.Join("%", queryString.Split("%").Select(x => NormalizeString(x))) + "%";
            var result = await conn.QueryAsync<SearchResult>(@"
                select track_name title, artist_name artist, album_title, release_year
                from tracks t
                join albums al on t.album_id = al.album_id
                join artists ar on t.artist_id = ar.artist_id
                where artist_title like @normalizedArtist
                limit 50
            ", new { normalizedArtist });
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
    }
}
