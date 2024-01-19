using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace MusicVideoJukebox.Core
{
    public class DeezerMetadataProvider
    {
        HttpClient httpClient;

        public DeezerMetadataProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<VideoInfo> GetMetadata(string artist, string track)
        {
            var videoInfo = new VideoInfo { Artist = artist, Title = track };
            var deezerResult = await GetFromDeezer(httpClient, videoInfo);
            if (deezerResult.Success)
            {
                videoInfo.Year = deezerResult.Year;
                videoInfo.Album = deezerResult.AlbumTitle;
            }
            return videoInfo;
        }


        static async Task<DeezerResult> GetFromDeezer(HttpClient client, VideoInfo info)
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
}
