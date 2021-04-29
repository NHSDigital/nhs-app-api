using System.Net.Http;
using System.Net.Http.Headers;
using NHSOnline.Backend.AspNet.HealthChecks;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public class MessagesHttpClient : INhsAppHealthCheckClient
    {
        private const string ApiKeyHeaderName = "x-api-key";
        private const string MediaType = "application/json";

        public HttpClient Client { get; }

        public MessagesHttpClient(HttpClient client, IMessagesApiConfig config)
        {
            client.BaseAddress = config.BaseUrl;
            client.DefaultRequestHeaders.Add(ApiKeyHeaderName, new[] { config.ApiKey });
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));

            Client = client;
        }
    }
}
