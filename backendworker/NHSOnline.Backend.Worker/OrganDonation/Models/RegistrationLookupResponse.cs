using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class RegistrationLookupResponse : RegistrationBase
    {
        public string OrganDonationDecision { get; set; }

        public string FaithDeclaration { get; set; }

        public Dictionary<string, string> DonationWishes { get; set; }
    }
}