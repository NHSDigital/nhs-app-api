namespace NHSOnline.Backend.Worker.Areas.Session
{
    public class SessionCreateResultVisitorOutput
    {
        public bool SessionWasCreated { get; set; }
        public int StatusCode { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public UserSession UserSession { get; set; }
    }
}