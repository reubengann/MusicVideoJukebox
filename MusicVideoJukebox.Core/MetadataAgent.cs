using Dapper;
using FuzzySharp;
using FuzzySharp.PreProcess;
using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

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

        public async Task GetMetadata()
        {
            var httpClient = new HttpClient();
            var rows = await dbConnection.QueryAsync<MetadataRow>("SELECT track_id, year, artist, album, track from songs;");
            var metadataRowMap = rows.ToDictionary(x => $"{x.artist} - {x.track}");
            foreach (var videoInfo in library.InfoMap.Values)
            {
                string target = $"{videoInfo.Artist} - {videoInfo.Title}";
                Console.WriteLine(target);
                var candidates = metadataRowMap.Keys.ToList();
                var searchResult = FuzzySearch.FuzzySearchStrings(target.ToLower(), candidates, 90);
                if (searchResult.Count == 0)
                {
                    Console.WriteLine($"Could not find metadata for {target} in database. Trying Deezer");
                    var deezerResult = await GetFromDeezer(httpClient, videoInfo);
                    if (deezerResult.Success)
                    {
                        videoInfo.Year = deezerResult.Year;
                        videoInfo.Album = deezerResult.AlbumTitle;
                        await dbConnection.ExecuteAsync("INSERT INTO songs (year, track, album, artist) values (@Year, @Track, @Album, @Artist)",
                            new
                            { Year = videoInfo.Year, Track = videoInfo.Title, Album = videoInfo.Album, Artist = videoInfo.Artist });
                    }
                    else
                    {
                        Console.WriteLine("Deezer failed to produce a result");
                    }
                    continue;
                }
                if (searchResult.Count > 1)
                    throw new NotImplementedException("Multiple matches");

                var md = metadataRowMap[searchResult[0].Item1];
                Console.WriteLine($"Likely album for {target} is {md.album}");
                videoInfo.Album = md.album;
                videoInfo.Year = md.year;
            }
        }

        async Task<DeezerResult> GetFromDeezer(HttpClient client, VideoInfo info)
        {
            var cleanQuery = HttpUtility.UrlEncode($"{info.Artist} {info.Title}");
            var response = await client.GetAsync($"https://api.deezer.com/search?q={cleanQuery}");
            if (!response.IsSuccessStatusCode) throw new Exception();
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonAsObject = JsonSerializer.Deserialize<JsonObject>(responseString) ?? throw new Exception();
            var searchResults = jsonAsObject["data"]?.AsArray() ?? throw new Exception();
            foreach (var item in searchResults)
            {
                await Task.Delay(1000);
                var itemAsObj = item?.AsObject() ?? throw new Exception();
                var album = itemAsObj["album"]?.AsObject() ?? throw new Exception();
                var albumTitle = album["title"]?.GetValue<string>() ?? throw new Exception();
                if (new[] { "hit", "best", "party", "karaoke" }.Any(c => albumTitle.ToLower().Contains(c))) continue;
                var albumId = album["id"]?.GetValue<int>() ?? throw new Exception();
                response = await client.GetAsync($"https://api.deezer.com/album/{albumId}");
                responseString = await response.Content.ReadAsStringAsync();
                var albumResponseAsObj = JsonSerializer.Deserialize<JsonObject>(responseString) ?? throw new Exception();
                var albumType = albumResponseAsObj["record_type"]?.AsValue().ToString() ?? throw new Exception();
                if (albumType != "album") continue;
                var releaseDate = albumResponseAsObj["release_date"]?.GetValue<string>() ?? throw new Exception();
                var year = int.Parse(releaseDate.Split("-").First());
                return new DeezerResult(true, year, albumTitle);
            }
            return new DeezerResult(false);
        }
    }

    class DeezerResult
    {
        public DeezerResult(bool success, int? year = null, string? albumTitle = null)
        {
            Success = success;
            Year = year;
            AlbumTitle = albumTitle;
        }

        public bool Success { get; set; }
        public int? Year { get; set; }
        public string? AlbumTitle { get; set; }
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
