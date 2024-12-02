namespace MusicVideoJukebox.Test.Fakes
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseGenerator;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseGenerator)
        {
            _responseGenerator = responseGenerator;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = _responseGenerator(request);
            return Task.FromResult(response);
        }
    }
}
