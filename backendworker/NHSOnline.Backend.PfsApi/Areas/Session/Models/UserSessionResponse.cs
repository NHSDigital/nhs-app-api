using System;

namespace NHSOnline.Backend.PfsApi.Areas.Session.Models
{
    public class UserSessionResponse
    {
        public string Name { get; set; }
        
        public int SessionTimeout { get; set; }

        public string OdsCode { get; set; }

        public string Token { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        public string NhsNumber { get; set; }

        public string AccessToken { get; set; }
        
        public bool Im1MessagingEnabled { get; set; }
    }
}