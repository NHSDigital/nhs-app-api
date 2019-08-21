using System.Net.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionLinkageHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.Vision;
        protected override string Provider => "Vision";

        public override HttpRequestIdentity Identify(HttpRequestMessage request)
        {
            return new HttpRequestIdentity(Provider, request, SourceApi);
        }
    }
}