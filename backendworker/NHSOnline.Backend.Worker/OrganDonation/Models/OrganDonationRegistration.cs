using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.Worker.GpSystems.OrganDonation.Models
{
    public class OrganDonationRegistration
    {
        public string Identifier { get; set; }

        public string NhsNumber { get; set; }
        
        public string NameFull { get; set; }
        
        public Name Name { get; set; }

        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }
        
        public string AddressFull { get; set; }

        public Address Address { get; set; }

        public string EmailAddress { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public Decision Decision { get; set; }

        public DecisionDetails DecisionDetails { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public FaithDeclaration? FaithDeclaration { get; set; }
    }
}