using System.Net.Http;

namespace NHSOnline.Backend.Worker.Brothermailer
{
    public class BrothermailerHttpClient
    {
        public HttpClient Client { get; }
        
        public BrothermailerHttpClient(HttpClient client, IBrothermailerConfig config)
        {
            client.BaseAddress = config.BrothermailerBaseUrl;
            Client = client;
            
        }
    }
}