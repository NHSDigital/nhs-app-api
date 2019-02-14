using System;

namespace NHSOnline.Backend.GpSystems.Linkage.Models
{
    public class GetLinkageRequest
    {
        public string NhsNumber { get; set; }
        public string Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string OdsCode { get; set; }
        public string IdentityToken { get; set; }
    }
}