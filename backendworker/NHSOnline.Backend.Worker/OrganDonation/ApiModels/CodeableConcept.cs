using System.Collections.Generic;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;

namespace NHSOnline.Backend.Worker.OrganDonation.ApiModels
{
    public class CodeableConcept
    {
        public List<Coding> Coding { get; set; }
    }
}