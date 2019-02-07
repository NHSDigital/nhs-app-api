using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.ApiModels
{
    public class Address
    {
        public List<string> Line { get; set; }

        public string PostalCode { get; set; }
    }
}