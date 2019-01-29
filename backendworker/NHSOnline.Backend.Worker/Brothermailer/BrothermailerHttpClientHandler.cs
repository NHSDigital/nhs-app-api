using System.Net.Http;

namespace NHSOnline.Backend.Worker.Brothermailer
{
    public class BrothermailerHttpClientHandler: HttpClientHandler
    {
        public BrothermailerHttpClientHandler()
        {
            AllowAutoRedirect = false;
        }
    }
}