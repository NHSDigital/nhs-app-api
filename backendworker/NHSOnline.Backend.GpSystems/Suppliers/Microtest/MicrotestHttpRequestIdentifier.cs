using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.Microtest;
        protected override string Provider => "Microtest";

        public override HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            return new HttpRequestIdentity(Provider, request, SourceApi);
        }
    }
}