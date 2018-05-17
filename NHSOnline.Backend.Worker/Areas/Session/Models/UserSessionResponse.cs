namespace NHSOnline.Backend.Worker.Areas.Session.Models
{
    public class UserSessionResponse
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public UserSession UserSession { get; set; }
    }
}