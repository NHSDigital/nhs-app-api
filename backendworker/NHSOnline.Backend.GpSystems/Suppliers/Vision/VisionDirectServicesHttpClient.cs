using System.Net.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionDirectServicesHttpClient
    {
        public VisionDirectServicesHttpClient(HttpClient client, IVisionDirectServicesConfig config)
        {
            client.BaseAddress = config.ApiUrl;
            Client = client;
        }

        public HttpClient Client { get; }
    }
}