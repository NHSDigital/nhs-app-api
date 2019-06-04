using System;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Models
{
    public class RetrieveLinkageKeysRequest
    {
        public string OdsCode { get; set; }

        public string Surname { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string NhsNumber { get; set; }

        public string EmailAddress { get; set; }

        public string IdentityToken { get; set; }
    }
}