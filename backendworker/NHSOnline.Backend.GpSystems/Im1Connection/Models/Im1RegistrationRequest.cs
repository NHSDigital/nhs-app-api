using System;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Models
{
    public class Im1RegistrationRequest
    {
        public string AccountId { get; set; }
        
        public string LinkageKey { get; set; }
        
        public string OdsCode { get; set; }

        public string Surname { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        public string NhsNumber { get; set; }
        
        public string EmailAddress { get; set; }
        
        public string IdentityToken { get; set; }
    }
}