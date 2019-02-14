using System.Net.Http;
using System.Net.Http.Headers;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionPFSHttpClient
    {
        private const string MediaType = "text/xml";

        public VisionPFSHttpClient(HttpClient client, IVisionPFSConfig config)
        {
            client.BaseAddress = config.ApiUrl;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            Client = client;
        }

        public HttpClient Client { get; }
    }
}