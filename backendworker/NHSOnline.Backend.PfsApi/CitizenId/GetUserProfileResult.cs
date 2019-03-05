using System.Net;
using NHSOnline.Backend.PfsApi.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.CitizenId
{
    public class GetUserProfileResult
    {
        public Option<UserProfile> UserProfile { get; set; }
        public HttpStatusCode StatusCode { get; set; }     
    }
}