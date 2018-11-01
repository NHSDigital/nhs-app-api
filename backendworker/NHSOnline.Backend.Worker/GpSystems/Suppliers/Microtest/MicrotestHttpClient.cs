using System.Net.Http;
using System.Net.Http.Headers;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest
{
    public class MicrotestHttpClient
    {
        private const string MediaType = "application/json";

        public MicrotestHttpClient(HttpClient client, IMicrotestConfig config)
        {
            client.BaseAddress = config.BaseUrl;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            Client = client;
        }

        public HttpClient Client { get; }
    }
}