using System.Net.Http;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public class ServiceJourneyRulesHttpClient
    {
        public HttpClient Client { get; }
        
        public ServiceJourneyRulesHttpClient(HttpClient client, IServiceJourneyRulesConfig config)
        {   
            client.BaseAddress = config.ServiceJourneyRulesBaseUrl;
            Client = client;
        }
    }
}