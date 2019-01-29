using System.Net.Http;

namespace NHSOnline.Backend.Worker.GpSearch
{
    public class GpLookupHttpClient
    {
        public HttpClient Client { get; }
        
        public GpLookupHttpClient(HttpClient client, IGpLookupConfig config)
        {
            client.BaseAddress = config.NhsSearchBaseUrl;
            Client = client;
        }
    }
}