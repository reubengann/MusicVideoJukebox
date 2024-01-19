using Dapper;
using System.Data;

namespace MusicVideoJukebox.Core
{

    // Used to populate the library database. Not for normal use
    public class LibraryFillInMetadataProvider
    {
        private readonly FuzzyMatchDatabaseMetadataProvider _fuzzy;
        private readonly DeezerMetadataProvider _deezer;
        private readonly IDbConnection libraryConnection;

        public LibraryFillInMetadataProvider(IDbConnection libraryConnection)
        {
            _fuzzy = new FuzzyMatchDatabaseMetadataProvider(libraryConnection);
            _deezer = new DeezerMetadataProvider(new HttpClient());
            this.libraryConnection = libraryConnection;
        }

        public async Task<VideoInfo> GetMetadata(string artist, string track)
        {
            var resultFromFuzzy = await _fuzzy.GetMetadata(artist, track);
            if (resultFromFuzzy.Album != null)
                return resultFromFuzzy;
            var videoInfo = await _deezer.GetMetadata(artist, track);
            if (videoInfo.Album != null)
            {
                await libraryConnection.ExecuteAsync("INSERT INTO songs (year, track, album, artist) values (@Year, @Track, @Album, @Artist)",
                        new
                        { Year = videoInfo.Year, Track = videoInfo.Title.Trim(), Album = videoInfo.Album?.Trim(), Artist = videoInfo.Artist });
            }
            return videoInfo;
        }
    }
}
