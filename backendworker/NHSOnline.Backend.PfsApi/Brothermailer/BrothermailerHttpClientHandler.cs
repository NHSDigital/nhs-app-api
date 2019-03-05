using System.Net.Http;

namespace NHSOnline.Backend.PfsApi.Brothermailer
{
    public class BrothermailerHttpClientHandler: HttpClientHandler
    {
        public BrothermailerHttpClientHandler()
        {
            AllowAutoRedirect = false;
        }
    }
}