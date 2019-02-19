using System.Collections.Generic;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class OrganDonationErrorResponse
    {
        public List<Issue> Issue { get; set; }
    }
}