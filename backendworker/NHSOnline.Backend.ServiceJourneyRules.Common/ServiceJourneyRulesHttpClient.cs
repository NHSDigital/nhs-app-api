using System.Net.Http;
using NHSOnline.Backend.AspNet.HealthChecks;

namespace NHSOnline.Backend.ServiceJourneyRules.Common
{
    public class ServiceJourneyRulesHttpClient : INhsAppHealthCheckClient
    {
        public HttpClient Client { get; }

        public ServiceJourneyRulesHttpClient(HttpClient client, IServiceJourneyRulesConfig config)
        {
            client.BaseAddress = config.ServiceJourneyRulesBaseUrl;
            Client = client;
        }
    }
}