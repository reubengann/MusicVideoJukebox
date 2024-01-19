using Dapper;
using FuzzySharp;
using FuzzySharp.PreProcess;
using System.Data;

namespace MusicVideoJukebox.Core
{
    public class FuzzyMatchDatabaseMetadataProvider
    {
        private readonly IDbConnection referenceConnection;
        HttpClient? httpClient;
        Dictionary<string, MetadataRow>? metadataRowMap;

        public FuzzyMatchDatabaseMetadataProvider(IDbConnection referenceConnection)
        {
            this.referenceConnection = referenceConnection;
        }

        public async Task<VideoInfo> GetMetadata(string artist, string track)
        {
            if (metadataRowMap == null)
            {
                var rows = await referenceConnection.QueryAsync<MetadataRow>("SELECT track_id, year, artist, album, track from songs;");
                metadataRowMap = rows.ToDictionary(x => $"{x.artist} - {x.track}");
            }

            if (httpClient == null)
                httpClient = new HttpClient();

            var videoInfo = new VideoInfo { Artist = artist, Title = track };

            string target = $"{artist} - {track}";
            var candidates = metadataRowMap.Keys.ToList();
            var searchResult = FuzzySearch.FuzzySearchStrings(target.ToLower(), candidates, 90);
            if (searchResult.Count == 0)
                return videoInfo;
            else
            {
                MetadataRow md;
                if (searchResult.Count > 1)
                {
                    Tuple<string, int> maxTuple = searchResult.OrderByDescending(t => t.Item2).First();
                    md = metadataRowMap[maxTuple.Item1];
                }
                else
                {
                    md = metadataRowMap[searchResult[0].Item1];
                }
                Console.WriteLine($"Likely album for {target} is {md.album}");
                videoInfo.Album = md.album;
                videoInfo.Year = md.year;
            }
            return videoInfo;
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
                int similarity = Fuzz.WeightedRatio(target, item, PreprocessMode.Full);

                if (similarity >= threshold)
                    results.Add(new Tuple<string, int>(item, similarity));
            }

            return results;
        }
    }
}
