
using System.Net.Http;

namespace NHSOnline.Backend.Worker
{
    public interface IHttpClientFactory
    {
        HttpClient Create();
    }
}
