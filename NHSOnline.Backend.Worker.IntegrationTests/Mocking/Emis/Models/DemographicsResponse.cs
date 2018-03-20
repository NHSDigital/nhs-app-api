using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models
{
    public class DemographicsResponse
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string CallingName { get; set; }
        public IList<PatientIdentifier> PatientIdentifiers { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Sex Sex { get; set; }
        public ContactDetails ContactDetails { get; set; }
        public Address Address { get; set; }
    }
}


