using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    internal class ReferenceData
    {
        public string Id{ get; set; }

        public List<Coding> Concept { get; set; }
    }
}