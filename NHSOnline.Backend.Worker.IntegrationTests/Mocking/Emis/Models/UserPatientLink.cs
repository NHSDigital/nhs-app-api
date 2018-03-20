namespace NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models
{
    public class UserPatientLink
    {
        public string UserPatientLinkToken { get; set; }
        public string PatientActivityContextGuid { get; set; }
        public string NationalPracticeCode { get; set; }
        public string Title { get; set; }
        public string Forenames { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public AssociationType AssociationType { get; set; }
    }
}