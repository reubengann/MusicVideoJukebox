using Dapper;
using FuzzySharp;
using FuzzySharp.PreProcess;
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
                var candidates = metadataRowMap.Keys.ToList();
                var searchResult = FuzzySearch.FuzzySearchStrings(target.ToLower(), candidates, 90);
                if (searchResult.Count == 0)
                {
                    Console.WriteLine($"Could not find metadata for {target} in database. Trying Deezer");
                    continue;
                }
                if (searchResult.Count > 1)
                    throw new NotImplementedException("Multiple matches");

                var md = metadataRowMap[searchResult[0].Item1];
                Console.WriteLine($"Likely album for {target} is {md.album}");
                row.Album = md.album;
                row.Year = md.year;
            }
        }

        void GetFromDeezer()
        {
            /*
            response = requests.get(f"https://api.deezer.com/search?q={clean_query}")
            data = response.json()
            for album_id, album_title in [(r['album']['id'], r['album']['title']) for r in data['data']]:
                response = requests.get(f"https://api.deezer.com/album/{album_id}")
                album_data = response.json()
                if album_data['record_type'] == 'album':
                    if is_nuisance(album_title):
                        continue
                    print(f"Probable album for {i[1].Artist} {i[1].Track} is {album_title}")
                    df.loc[i[0], 'Album'] = album_title
                    break
                time.sleep(1)
             */
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
