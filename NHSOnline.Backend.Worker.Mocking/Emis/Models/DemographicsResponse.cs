using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class DemographicsResponse
    {
        public string Title { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string CallingName { get; }
        public IEnumerable<PatientIdentifier> PatientIdentifiers { get; }
        public string DateOfBirth { get; }
        public Sex Sex { get; }
        public ContactDetails ContactDetails { get; }
        public Address Address { get; }

        public DemographicsResponse(string title, string firstName, string surname, IEnumerable<PatientIdentifier> patientIdentifiers)
        {
            Title = title;
            FirstName = firstName;
            Surname = surname;
            CallingName = firstName;
            PatientIdentifiers = patientIdentifiers.ToArray();
            DateOfBirth = "1919-12-24T14:03:15.892Z";
            Sex = Sex.NotSpecified;
            ContactDetails = new ContactDetails();
            Address = new Address();
        }
    }
}


