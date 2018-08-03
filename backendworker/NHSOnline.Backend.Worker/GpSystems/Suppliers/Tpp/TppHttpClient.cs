using System.Net.Http;
using System.Net.Http.Headers;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppHttpClient
    {
        public const string MediaType = "text/xml";

        public TppHttpClient(HttpClient client, ITppConfig config)
        {
            client.BaseAddress = config.ApiUrl;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
