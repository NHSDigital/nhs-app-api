using System.Net.Http;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public class CitizenIdHttpClient
    {
        public CitizenIdHttpClient(HttpClient client, ICitizenIdConfig config)
        {
            client.BaseAddress = config.CitizenIdApiBaseUrl;
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
