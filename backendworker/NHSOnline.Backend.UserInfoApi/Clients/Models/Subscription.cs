namespace NHSOnline.Backend.UserInfoApi.Clients.Models
{
    public class Subscription
    {
        public string Email { get; set; }
        public string ExtRef { get; set; }
        public EmbeddedData EmbeddedData { get; set; }
    }
}