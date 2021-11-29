namespace NHSOnline.Backend.UsersApi.Notifications.Models
{
    public class NotificationResponse
    {
        public string NotificationId { get; set; }

        public string TrackingId { get; set; }

        public bool Scheduled { get; set; }
    }
}