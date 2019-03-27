using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models
{
    public class DemographicsGetResponse
    {
        public string Title { get; set; }
        public string Surname { get; set; }
        public string Forenames1 { get; set; }
        public string Forenames2 { get; set; }
        public DateTime Dob { get; set; }
        public string Sex { get; set; }
        public string Nhs { get; set; }
        public string HouseName { get; set; }
        public string RoadName { get; set; }
        public string Locality { get; set; }
        [JsonProperty("post_town")]
        public string PostTown { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Email { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
    }
}
