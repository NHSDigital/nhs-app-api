using System.Net;
using NHSOnline.Backend.Worker.CitizenId.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public class GetUserProfileResult
    {
        public Option<UserProfile> UserProfile { get; set; }
        public HttpStatusCode StatusCode { get; set; }     
    }
}