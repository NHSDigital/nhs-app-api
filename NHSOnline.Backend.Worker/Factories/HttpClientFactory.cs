using System.Net.Http;

namespace NHSOnline.Backend.Worker.Factories
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}