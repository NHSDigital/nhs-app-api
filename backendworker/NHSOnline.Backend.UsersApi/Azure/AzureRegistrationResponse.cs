using System;

namespace NHSOnline.Backend.UsersApi.Azure
{
    public class AzureRegistrationResponse
    {
        public string RegistrationId { get; set; }
        public DateTime? RegistrationExpiry { get; set; }
    }
}