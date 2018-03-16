using System.Net.Http;

namespace NHSOnline.Backend.Worker
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}