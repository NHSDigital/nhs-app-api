using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.ApiModels
{
    internal class ReferenceData
    {
        public string Id{ get; set; }

        public List<Coding> Concept { get; set; }
    }
}