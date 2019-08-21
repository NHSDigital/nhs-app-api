using System.Net.Http;

namespace NHSOnline.Backend.Support.Http
{
    public abstract class HttpRequestIdentifier : IHttpRequestIdentifier
    {
        protected abstract SourceApi SourceApi { get; }
        protected abstract string Provider { get; }

        public virtual HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            return new HttpRequestIdentity(Provider, request, SourceApi);
        }
    }
}