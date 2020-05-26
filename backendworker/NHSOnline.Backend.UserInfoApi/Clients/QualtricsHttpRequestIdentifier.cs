using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.UserInfoApi.Clients
{
    public class QualtricsHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.Qualtrics;
        protected override string Provider => "Qualtrics";
    }
}