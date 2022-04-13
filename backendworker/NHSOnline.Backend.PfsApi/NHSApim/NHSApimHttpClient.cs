using System.Net.Http;

namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public class NhsApimHttpClient
    {
        public NhsApimHttpClient(HttpClient client, INhsApimConfig config)
        {
            client.BaseAddress = config.BaseUrl;
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
