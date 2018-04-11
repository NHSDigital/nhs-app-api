using System;

namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class UserPatientLink
    {
        public string UserPatientLinkToken { get; }
        public string PatientActivityContextGuid { get; }
        public string NationalPracticeCode { get; }
        public string Title { get; }
        public string Forenames { get; }
        public string Surname { get; }
        public int Age { get; }
        public AssociationType AssociationType { get; }

        public UserPatientLink(string title, string firstName, string surname, string userPatientLinkToken, string odsCode, AssociationType associationType)
        {
            UserPatientLinkToken = userPatientLinkToken;
            PatientActivityContextGuid = Guid.NewGuid().ToString();
            NationalPracticeCode = odsCode;
            Title = title;
            Forenames = firstName;
            Surname = surname;
            Age = 42;
            AssociationType = associationType;
        }
    }
}