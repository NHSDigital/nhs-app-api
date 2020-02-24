namespace NHSOnline.Backend.Auth.CitizenId.Models
{
    public class IdToken
    {
        public string Subject { get; set; }

        public string Jti { get; set; }
    }
}