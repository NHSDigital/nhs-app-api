using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.UsersApi.Areas.Devices.Models
{
    public class NotificationSendRequest
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Uris are not serializable")]
        public string Url { get; set; }
    }
}