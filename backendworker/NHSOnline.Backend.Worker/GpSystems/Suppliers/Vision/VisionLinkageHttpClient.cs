using System.Net.Http;
using System.Net.Http.Headers;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionLinkageHttpClient
    {
        private const string MediaType = "application/json";

        public VisionLinkageHttpClient(HttpClient client, IVisionLinkageConfig config)
        {
            client.BaseAddress = config.ApiUrl;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            Client = client;
        }

        public HttpClient Client { get; }
    }
}