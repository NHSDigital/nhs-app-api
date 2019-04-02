using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public class SessionLogoffResultVisitorOutput
    {
        public bool SessionWasDeleted { get; set; }
        public int StatusCode { get; set; }
        public GpUserSession GpUserSession { get; set; }
    }
}