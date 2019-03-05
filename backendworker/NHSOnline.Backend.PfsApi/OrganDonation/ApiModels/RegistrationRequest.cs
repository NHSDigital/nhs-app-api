using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSOnline.Backend.PfsApi.OrganDonation.ApiModels
{
    internal class RegistrationRequest : RegistrationBase
    {
        [Required]
        public CodeableConcept EthnicCategory { get; set; }

        [Required]
        public CodeableConcept ReligiousAffiliation { get; set; }

        [Required]
        public string OrganDonationDecision { get; set; }

        public string FaithDeclaration { get; set; }

        public Dictionary<string, string> DonationWishes { get; set; }
    }
}