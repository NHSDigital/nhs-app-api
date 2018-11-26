using System.Net.Http;

namespace NHSOnline.Backend.Worker.Support.Http
{
    public interface IHttpRequestIdentifier
    {
        HttpRequestIdentity Identify(HttpRequestMessage request);
    }
}