using System.Net;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class GetUserProfileResult
    {
        public Option<UserProfile> UserProfile { get; set; }
        public HttpStatusCode StatusCode { get; set; }     
    }
}