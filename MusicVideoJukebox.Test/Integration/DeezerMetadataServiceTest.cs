using MusicVideoJukebox.Core.Metadata;

namespace MusicVideoJukebox.Test.Integration
{
    public class DeezerMetadataServiceTest
    {
        DeezerMetadataProvider dut;

        public DeezerMetadataServiceTest()
        {
            dut = new DeezerMetadataProvider(new HttpClient());
        }

        [Fact]
        public async Task GetsFromWeb()
        {
            var results = await dut.GetMetadataCandidates("Annie Lennox", "Why");
            Assert.Equal(25, results.Count);
            var result = results[0];
            Assert.Equal("Why", result.SongTitle);
        }
    }
}
