namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class SessionLogoffResultVisitorOutput
    {
        public bool SessionWasDeleted { get; set; }
        public int StatusCode { get; set; }
        public string Name { get; set; }
        public int SessionTimeout { get; set; }
        public UserSession UserSession { get; set; }
    }
}