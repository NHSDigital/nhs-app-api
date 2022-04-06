using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.SecondaryCareAggregator;

        protected override string Provider => "SecondaryCareAggregator";
    }
}