using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class RegistrationCreateRequest : RegistrationBase
    {
        public CodeableConcept EthnicCategory { get; set; }

        public CodeableConcept ReligiousAffiliation { get; set; }

        public string OrganDonationDecision { get; set; }

        public string FaithDeclaration { get; set; }

        public Dictionary<string, string> DonationWishes { get; set; }
    }
}