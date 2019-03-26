using System.Net.Http;

namespace NHSOnline.Backend.NominatedPharmacy.Clients
{
    public class NominatedPharmacyHttpClient
    {
        public NominatedPharmacyHttpClient(HttpClient client, INominatedPharmacyConfig config)
        {
            client.BaseAddress = config.BaseUrl;
            Client = client;
        }

        public HttpClient Client { get; }
    }
}