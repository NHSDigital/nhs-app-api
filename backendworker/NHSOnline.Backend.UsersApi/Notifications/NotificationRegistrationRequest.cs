using System.Collections.Generic;
using NHSOnline.Backend.UsersApi.Areas.Devices.Models;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class NotificationRegistrationRequest
    {
        public DeviceType? DeviceType { get; set; }
        
        public string DevicePns { get; set; }
        
        public string NhsLoginId { get; set; }
    }
}