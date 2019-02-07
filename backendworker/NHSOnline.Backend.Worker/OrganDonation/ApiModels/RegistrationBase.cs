using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.OrganDonation.ApiModels
{
    public abstract class RegistrationBase
    {
        public string Id { get; set; }

        public string ResourceType { get; } = "Registration";

        public List<Identifier> Identifier { get; set; }

        public List<Name> Name { get; set; }
        
        public string Gender { get; set; }

        public string BirthDate { get; set; }

        public List<Address> Address { get; set; }

        public List<Identifier> Telecom { get; set; }
    }
}