using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class ReferenceDataResponse
    {
        public string Id{ get; set; }

        public List<Coding> Concept { get; set; }
    }
}