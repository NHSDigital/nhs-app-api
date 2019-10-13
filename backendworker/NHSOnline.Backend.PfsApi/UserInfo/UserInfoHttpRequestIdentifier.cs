using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class UserInfoHttpRequestIdentifier : HttpRequestIdentifier
    {
        protected override SourceApi SourceApi => SourceApi.UserInfo;
        protected override string Provider => "UserInfo";
    }
}