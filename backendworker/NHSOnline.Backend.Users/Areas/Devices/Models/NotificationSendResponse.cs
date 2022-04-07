namespace NHSOnline.Backend.Users.Areas.Devices.Models
{
    public class NotificationSendResponse
    {
        public string NotificationId { get; set; }

        public bool Scheduled { get; set; }

        public string HubPath { get; set; }
    }
}