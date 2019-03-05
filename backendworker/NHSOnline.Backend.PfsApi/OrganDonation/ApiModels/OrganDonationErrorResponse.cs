using System.Collections.Generic;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;

namespace NHSOnline.Backend.PfsApi.OrganDonation.ApiModels
{
    public class OrganDonationErrorResponse
    {
        public List<Issue> Issue { get; set; }
    }
}