using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.OrganDonation.ApiModels
{
    internal class Registration : RegistrationBase
    {
        public string OrganDonationDecision { get; set; }

        public string FaithDeclaration { get; set; }

        public Dictionary<string, string> DonationWishes { get; set; }
    }
}