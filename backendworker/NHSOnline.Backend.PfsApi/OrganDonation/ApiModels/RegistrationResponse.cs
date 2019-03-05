using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.OrganDonation.ApiModels
{
    internal class RegistrationResponse
    {
        public string Id { get; set; }
        public List<Issue> Issue { get; set; }
    }
}