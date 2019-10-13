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

        public override bool HasBadRequestResponse => StatusCode.IsBadRequestCode();
            
        public override string ErrorForLogging => $"Error Code: '{StatusCode}'. ";
        
        protected override bool FormatResponseIfUnsuccessful => false;
    }
}