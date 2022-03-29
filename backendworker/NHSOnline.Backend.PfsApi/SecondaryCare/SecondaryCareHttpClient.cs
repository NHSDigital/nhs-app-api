using System.Net.Http;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareHttpClient
    {
        public SecondaryCareHttpClient(HttpClient client, ISecondaryCareConfig config)
        {
            client.BaseAddress = config.BaseUrl;
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
