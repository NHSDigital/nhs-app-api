using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.OrganDonation.ApiModels
{
    public class Name
    {
        public List<string> Prefix { get; set; }

        public List<string> Given { get; set; }

        public string Family { get; set; }

    }
}