using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class RegistrationLookupRequest
    {
        [IgnoreDataMember]
        [Required]
        public string NhsNumber { get; set; }

        [Required]
        public string Identifier => $"{IdentifierSystem}|{NhsNumber}";

        [Required]
        public string Given { get; set; }

        [Required]
        public string Family { get; set; }

        [Required]
        public string BirthDate { get; set; }
    }
}