using System.Net.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisHttpClient
    {
        public const string HeaderApplicationId = "X-API-ApplicationId";
        public const string HeaderVersion = "X-API-Version";

        public EmisHttpClient(HttpClient client, EmisConfigurationSettings config)
        {
            client.DefaultRequestHeaders.Add(HeaderApplicationId, config.ApplicationId);
            client.DefaultRequestHeaders.Add(HeaderVersion, config.Version);
            client.BaseAddress = config.BaseUrl;
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
