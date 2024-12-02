using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace MusicVideoJukebox.Core.Metadata
{
    public class DeezerMetadataProvider : IMetadataWebService
    {
        readonly HttpClient httpClient;

        public DeezerMetadataProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        //public async Task<MetadataGetResult> GetMetadata(string artist, string track)
        //{
        //    var deezerResult = await GetFromDeezer(httpClient, artist, track);
        //    return deezerResult;
        //}


        //private async Task<MetadataGetResult> GetFromDeezer(HttpClient client, string artist, string track)
        //{
        //    var cleanQuery = HttpUtility.UrlEncode($"{artist} {track}");
        //    var response = await client.GetAsync($"https://api.deezer.com/search?q={cleanQuery}");
        //    if (!response.IsSuccessStatusCode) throw new Exception();
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var jsonAsObject = JsonSerializer.Deserialize<JsonObject>(responseString) ?? throw new Exception();
        //    var searchResults = jsonAsObject["data"]?.AsArray() ?? throw new Exception();
        //    foreach (var item in searchResults)
        //    {
        //        await Task.Delay(1000); // rate limit
        //        var itemAsObj = item?.AsObject() ?? throw new Exception();
        //        var album = itemAsObj["album"]?.AsObject() ?? throw new Exception();
        //        var albumTitle = album["title"]?.GetValue<string>() ?? throw new Exception();
        //        if (new[] { "hit", "best", "party", "karaoke" }.Any(c => albumTitle.ToLower().Contains(c))) continue;
        //        var albumId = album["id"]?.GetValue<int>() ?? throw new Exception();
        //        response = await client.GetAsync($"https://api.deezer.com/album/{albumId}");
        //        responseString = await response.Content.ReadAsStringAsync();
        //        var albumResponseAsObj = JsonSerializer.Deserialize<JsonObject>(responseString) ?? throw new Exception();
        //        var albumType = albumResponseAsObj["record_type"]?.AsValue().ToString() ?? throw new Exception();
        //        if (albumType != "album") continue;
        //        var releaseDate = albumResponseAsObj["release_date"]?.GetValue<string>() ?? throw new Exception();
        //        var year = int.Parse(releaseDate.Split("-").First());
        //        return new MetadataGetResult { Success = true, Year = year, AlbumTitle = albumTitle };
        //    }
        //    return new MetadataGetResult { Success = false };
        //}

        public async Task<List<MetadataCandidate>> GetMetadataCandidates(string artist, string track)
        {
            var cleanQuery = HttpUtility.UrlEncode($"{artist} {track}");
            var response = await httpClient.GetAsync($"https://api.deezer.com/search?q={cleanQuery}");
            if (!response.IsSuccessStatusCode) throw new Exception();
            var responseString = await response.Content.ReadAsStringAsync();
            var searchResults = JsonSerializer.Deserialize<DeezerSearchResponse>(responseString) ?? throw new Exception();
            List<MetadataCandidate> candidates = [];
            foreach (var item in searchResults.Data)
            {
                candidates.Add(new MetadataCandidate
                {
                    AlbumId = item.Album.Id,
                    AlbumTitle = item.Album.Title,
                    ArtistName = item.Artist.Name,
                    SongTitle = item.Title
                }
                );
            }
            return candidates;
        }
    }

    class DeezerSearchResponse
    {
        [JsonPropertyName("data")]
        required public List<DeezerCandidate> Data { get; set; }
    }

    class Album
    {
        [JsonPropertyName("id")]
        required public int Id { get; set; }

        [JsonPropertyName("title")]
        required public string Title { get; set; }
    }

    class Artist
    {
        [JsonPropertyName("name")]
        required public string Name { get; set; }
    }

    class DeezerCandidate
    {
        [JsonPropertyName("title")]
        required public string Title { get; set; }

        [JsonPropertyName("artist")]
        required public Artist Artist { get; set; }

        [JsonPropertyName("album")]
        required public Album Album { get; set; }
    }
}
