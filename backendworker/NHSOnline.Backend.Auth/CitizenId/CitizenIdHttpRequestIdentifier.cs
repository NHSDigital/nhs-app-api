using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.NhsLogin;
        protected override string Provider => "CitizenId";
    }
}