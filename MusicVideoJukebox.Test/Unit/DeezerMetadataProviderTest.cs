using MusicVideoJukebox.Core.Metadata;
using MusicVideoJukebox.Test.Fakes;
using System.Net;
using System.Text;

namespace MusicVideoJukebox.Test.Unit
{
    public class DeezerMetadataProviderTest
    {
        DeezerMetadataProvider dut;
        HttpClient httpClient;
        FakeHttpMessageHandler httpMessageHandler;

        public DeezerMetadataProviderTest()
        {
            var searchJsonPath = Path.Combine(Directory.GetCurrentDirectory(), "sample_annie_lenox_why.json");
            var searchResponse = File.ReadAllText(searchJsonPath);

            httpMessageHandler = new FakeHttpMessageHandler(request =>
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(searchResponse, Encoding.UTF8, "application/json")
                };
            });
            httpClient = new HttpClient(httpMessageHandler);
            dut = new DeezerMetadataProvider(httpClient);
        }

        [Fact]
        public async Task GetsCandidates()
        {
            var results = await dut.GetMetadataCandidates("", "");
            Assert.Equal(25, results.Count);
            var result = results[0];
            Assert.Equal("Why", result.SongTitle);
        }
    }
}
