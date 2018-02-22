
using System.Net.Http;

namespace NHSOnline.Backend.Worker.Interfaces
{
    public interface IHttpClientFactory
    {
        HttpClient Create();
    }
}
