using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.ServiceJourneyRules.Common
{
    public class ServiceJourneyRulesHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.ServiceJourneyRules;
        protected override string Provider => "ServiceJourneyRules";
    }
}