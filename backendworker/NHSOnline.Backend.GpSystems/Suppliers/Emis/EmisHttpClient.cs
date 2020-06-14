using System.Net.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisHttpClient
    {
        private const string HeaderApplicationId = "X-API-ApplicationId";
        private const string HeaderVersion = "X-API-Version";

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
