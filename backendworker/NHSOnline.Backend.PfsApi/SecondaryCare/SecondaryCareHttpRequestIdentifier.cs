using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.SecondaryCare;

        protected override string Provider => "SecondaryCare";
    }
}