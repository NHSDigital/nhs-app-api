using System.Net;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.UserInfoApi.Clients
{
    public class UserResearchClientResponse : ApiResponse
    {
        public UserResearchClientResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();
        protected override bool FormatResponseIfUnsuccessful { get; }
    }
}