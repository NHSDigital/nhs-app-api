namespace NHSOnline.Backend.UsersApi.Areas.Devices.Models
{
    public class NotificationSendRequest
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public string Url { get; set; }
    }
}