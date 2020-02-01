using System.Net;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.PfsApi.UserInfo
{
    public class UserInfoResponse: ApiResponse
    {
        public UserInfoResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

        protected override bool FormatResponseIfUnsuccessful => false;
    }
}