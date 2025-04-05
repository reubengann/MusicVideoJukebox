using Dapper;
using MusicVideoJukebox.Core.Metadata;
using System.Data.SQLite;

namespace MusicVideoJukebox.Test.Integration
{
    public class ReferenceDataRepoTest : IDisposable
    {
        ReferenceDataRepo dut;
        //const string connectionString = @"Data Source=c:\repos\reference_test.db;Pooling=false;";

        public ReferenceDataRepoTest()
        {
            dut = new ReferenceDataRepo(@"c:\repos\reference_test.db");
            //EnsureTestDbCreated();
        }

        public void Dispose()
        {
            //using var conn = new SQLiteConnection(connectionString);
            //conn.Execute("DELETE FROM tracks");
            //conn.Execute("DELETE FROM albums");
            //conn.Execute("DELETE FROM artists");
        }

        [Fact]
        public async Task CanGetPartialMatch()
        {
            //WithTrack("Annie Lennox", "Diva", "Why", 1992);
            var result = await dut.SearchReferenceDb("Robert Miles%Children");
            Assert.NotEmpty(result);
            Assert.Equal("Children (Full Length)", result[0].Title);
        }

        [Fact]
        public async Task NormalizesDiacritics()
        {
            //WithTrack("Annie Lennox", "Diva", "Why", 1992);
            var result = await dut.SearchReferenceDb("Mýa%Pussycats");
            Assert.NotEmpty(result);
            Assert.Equal("Pussycats", result[0].Title);
        }

        [Fact]
        public async Task RemovesSpecialCharacters()
        {
            //WithTrack("Annie Lennox", "Diva", "Why", 1992);
            var result = await dut.SearchReferenceDb("AC/DC%Dirty Deeds");
            Assert.NotEmpty(result);
            Assert.Equal("Dirty Deeds Done Dirt Cheap", result[0].Title);
        }

        //[Fact]
        //public async Task CanMatchDifferentCase()
        //{
        //    WithTrack("Billie Eilish", "When We All Fall Asleep, Where Do We Go?", "bad guy", 2019);
        //    var result = await dut.TryGetExactMatch("Billie Eilish", "Bad Guy");
        //    Assert.True(result.Success);
        //    Assert.NotNull(result.FetchedMetadata);
        //    var md = result.FetchedMetadata;
        //    Assert.Equal(2019, md.FirstReleaseDateYear);
        //    Assert.Equal("When We All Fall Asleep, Where Do We Go?", md.AlbumTitle);
        //}

        //[Fact]
        //public async Task CanMatchThe()
        //{
        //    WithTrack("The Beatles", "Help!", "Yesterday", 1965);
        //    var result = await dut.TryGetExactMatch("Beatles", "Yesterday");
        //    Assert.True(result.Success);
        //    Assert.NotNull(result.FetchedMetadata);
        //    var md = result.FetchedMetadata;
        //    Assert.Equal(1965, md.FirstReleaseDateYear);
        //    Assert.Equal("Help!", md.AlbumTitle);
        //}

        //[Fact]
        //public async Task CanGetCandidates()
        //{
        //    WithTrack("AC/DC", "Dirty Deeds Done Dirty Cheap", "For Those About To Rock (We Salute You)", 1976);
        //    var result = await dut.GetCandidates("AC DC", "For those about to rock", 2);
        //    Assert.Single(result);
        //    Assert.Equal("AC/DC", result[0].ArtistName);
        //}

        //void WithTrack(string artist, string album, string track, int releaseYear)
        //{
        //    using var conn = new SQLiteConnection(connectionString);

        //    var artistId = conn.ExecuteScalar<int>(@"
        //    INSERT INTO artists (artist_name) values (@artist) returning artist_id
        //    ", new { artist });
        //    var albumId = conn.ExecuteScalar<int>(@"
        //        INSERT INTO albums (artist_id, album_title, primary_type_id, secondary_type_id, first_release_date_year)
        //        VALUES (@artistId, @album, 1, null, @releaseYear) returning album_id
        //    ", new {artistId, album, releaseYear});
        //    conn.Execute(@"
        //        INSERT INTO tracks (track_name, album_id) VALUES ( @track, @albumId )
        //    ", new {track, albumId});
        //}

        //void EnsureTestDbCreated()
        //{
        //    using var conn = new SQLiteConnection(connectionString);
        //    conn.Execute(@"
        //        CREATE TABLE IF NOT EXISTS artists (
        //            artist_id INTEGER PRIMARY KEY,
        //            artist_name TEXT
        //        );
        //    ");
        //    conn.Execute(@"
        //        CREATE TABLE IF NOT EXISTS albums (
        //            album_id INTEGER PRIMARY KEY,
        //            artist_id INTEGER,
        //            album_title TEXT,
        //            primary_type_id INTEGER,
        //            secondary_type_id INTEGER,
        //            first_release_date_year INTEGER
        //        );
        //    ");
        //    conn.Execute(@"
        //        CREATE TABLE IF NOT EXISTS tracks (
        //            track_id INTEGER PRIMARY KEY,
        //            track_name TEXT,
        //            album_id INTEGER
        //        );
        //    ");
        //}


    }
}
