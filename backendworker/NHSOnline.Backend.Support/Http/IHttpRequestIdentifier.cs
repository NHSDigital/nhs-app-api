using System.Net.Http;

namespace NHSOnline.Backend.Support.Http
{
    public interface IHttpRequestIdentifier
    {
        HttpRequestIdentity Identify(HttpRequestMessage request);
    }
}