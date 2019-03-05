using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public class GpSessionCreateResultVisitorOutput
    {
        public bool SessionWasCreated { get; set; }
        public int StatusCode { get; set; }
        public string Name { get; set; }
        public GpUserSession UserSession { get; set; }
    }
}