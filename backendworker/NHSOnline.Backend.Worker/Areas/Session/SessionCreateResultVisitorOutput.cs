using NHSOnline.Backend.Worker.GpSystems;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class GpSessionCreateResultVisitorOutput
    {
        public bool SessionWasCreated { get; set; }
        public int StatusCode { get; set; }
        public string Name { get; set; }
        public GpUserSession UserSession { get; set; }
    }
}