using System.Net;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdApiObjectResponse<TBody> : CitizenIdApiResponse
    {
        public CitizenIdApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public TBody Body { get; set; }
    }
}