using System.Net.Http;

namespace NHSOnline.Backend.UserInfoApi.Clients
{
    public class QualtricsHttpClient
    {
        private const string ApiTokenKey = "X-API-TOKEN";

        public QualtricsHttpClient(HttpClient client, IQualtricsConfig config)
        {
            client.DefaultRequestHeaders.Add(ApiTokenKey, config.Token);
            client.BaseAddress = config.BaseUrl;
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
