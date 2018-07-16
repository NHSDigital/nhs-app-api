using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionHttpClient
    {
        public const string MediaType = "text/xml";

        public VisionHttpClient(HttpClient client, IVisionConfig config)
        {
            client.BaseAddress = new Uri(config.ApiUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            Client = client;
        }

        public HttpClient Client { get; }
    }
}