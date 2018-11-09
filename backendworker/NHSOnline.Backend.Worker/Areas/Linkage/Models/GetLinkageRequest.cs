using System;

namespace NHSOnline.Backend.Worker.Areas.Linkage.Models
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