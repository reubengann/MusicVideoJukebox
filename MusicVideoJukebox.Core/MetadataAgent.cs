using Dapper;
using FuzzySharp;
using System.Data;

namespace MusicVideoJukebox.Core
{
    public class MetadataAgent
    {
        private readonly IDbConnection dbConnection;
        private readonly VideoLibrary library;

        public MetadataAgent(IDbConnection dbConnection, VideoLibrary library)
        {
            this.dbConnection = dbConnection;
            this.library = library;
        }

        public void GetMetadata()
        {
            var rows = dbConnection.Query<MetadataRow>("SELECT track_id, year, artist, album, track from songs;");
            var metadataRowMap = rows.ToDictionary(x => $"{x.artist} - {x.track}");
            foreach (var row in library.InfoMap.Values)
            {
                string target = $"{row.Artist} - {row.Title}";
                Console.WriteLine(target);
                var searchResult = FuzzySearch.FuzzySearchStrings(target, metadataRowMap.Keys.ToList(), 90);
                if (searchResult.Count != 1)
                    throw new NotImplementedException();
                var md = metadataRowMap[searchResult[0].Item1];
                Console.WriteLine($"Likely album for {target} is {md.album}");
                break;
            }
        }
    }

    class MetadataRow
    {
        public int track_id { get; set; }
        public int year { get; set; }
        public string artist { get; set; } = null!;
        public string? album { get; set; }
        public string track { get; set; } = null!;
    }


    public class FuzzySearch
    {
        public static List<Tuple<string, int>> FuzzySearchStrings(string target, List<string> collection, int threshold)
        {
            List<Tuple<string, int>> results = new List<Tuple<string, int>>();

            foreach (string item in collection)
            {
                int similarity = Fuzz.WeightedRatio(target, item);

                if (similarity >= threshold)
                    results.Add(new Tuple<string, int>(item, similarity));
            }

            return results;
        }
    }
}
