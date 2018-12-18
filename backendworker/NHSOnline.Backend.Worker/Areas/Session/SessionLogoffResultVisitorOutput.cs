using NHSOnline.Backend.Worker.GpSystems;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class SessionLogoffResultVisitorOutput
    {
        public bool SessionWasDeleted { get; set; }
        public int StatusCode { get; set; }
        public UserSession UserSession { get; set; }
    }
}