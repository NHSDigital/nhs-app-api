using System;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public class UserDevice
    {
        public string NhsLoginId { get; set; }
        public string DeviceId { get; set; }
        public string RegistrationId { get; set; }
        public DateTime? RegistrationExpiry { get; set; }
        public string PnsToken { get; set; }
    }
}