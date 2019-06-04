using System.Net.Http;
using System.Net.Http.Headers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestHttpClient
    {
        private const string MediaType = "application/json";

        public MicrotestHttpClient(HttpClient client, MicrotestConfigurationSettings configurationSettings)
        {
            client.BaseAddress = configurationSettings.BaseUrl;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            Client = client;
        }

        public HttpClient Client { get; }
    }
}