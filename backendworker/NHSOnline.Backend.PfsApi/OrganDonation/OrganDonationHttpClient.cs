using System.Net.Http;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public class OrganDonationHttpClient
    {
        private const string ClientIdHeaderKey = "X-Client-ID";
        private const string SubscriptionHeaderKey = "Ocp-Apim-Subscription-Key";

        public OrganDonationHttpClient(HttpClient client, IOrganDonationConfig config)
        {
            client.DefaultRequestHeaders.Add(ClientIdHeaderKey, config.ClientIdHeader);
            client.DefaultRequestHeaders.Add(SubscriptionHeaderKey, config.SubscriptionHeader);
            client.BaseAddress = config.BaseUrl;
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
