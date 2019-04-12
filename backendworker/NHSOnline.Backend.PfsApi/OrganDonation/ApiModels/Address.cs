using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.OrganDonation.ApiModels
{
    public class Address
    {
        public List<string> Line { get; set; }
        
        public string City { get; set; }
        
        public string District { get; set; }

        public string PostalCode { get; set; }
    }
}