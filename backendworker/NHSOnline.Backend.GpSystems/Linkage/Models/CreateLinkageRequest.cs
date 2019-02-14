using System;

namespace NHSOnline.Backend.GpSystems.Linkage.Models
{
    public class CreateLinkageRequest
    {
        public string OdsCode { get; set; }

        public string NhsNumber { get; set; }

        public string IdentityToken { get; set; }

        public string EmailAddress { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        public string Surname { get; set; }
    }
}
