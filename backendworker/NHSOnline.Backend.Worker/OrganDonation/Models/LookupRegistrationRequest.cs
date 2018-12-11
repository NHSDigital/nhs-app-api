using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class LookupRegistrationRequest
    {
        [IgnoreDataMember]
        [Required]
        public string NhsNumber { get; set; }

        [Required]
        public string Identifier => $"https://fhir.nhs.uk/Id/nhs-number|{NhsNumber}";

        [Required]
        public string Given { get; set; }

        [Required]
        public string Family { get; set; }

        [Required]
        public string BirthDate { get; set; }
    }
}