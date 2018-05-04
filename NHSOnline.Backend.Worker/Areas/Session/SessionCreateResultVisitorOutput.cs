using NHSOnline.Backend.Worker.Areas.Session.Models;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class SessionCreateResultVisitorOutput
    {
        public bool SessionWasCreated { get; set; }
        public int StatusCode { get; set; }
        public UserSessionResponse UserSessionResponse { get; set; }
    }
}