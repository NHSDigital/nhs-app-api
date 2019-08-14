using System;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class NotificationRegistrationResult
    {
        public string RegistrationId { get; set; }
        public DateTime? RegistrationExpiry { get; set; }
    }
}