using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.ApiModels
{
    internal class RegistrationResponse
    {
        public string Id { get; set; }
        public List<Issue> Issue { get; set; }
    }
}