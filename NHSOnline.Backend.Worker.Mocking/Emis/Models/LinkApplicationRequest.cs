namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class LinkApplicationRequest
    {
        public string Surname { get; set; }
        public string DateOfBirth { get; set; }
        public LinkageDetails LinkageDetails { get; set; }

        public LinkApplicationRequest(string surname, string dateOfBirth, string accountId,
            string linkageKey, string nationalPracticeCode)
        {
            Surname = surname;
            DateOfBirth = dateOfBirth;
            LinkageDetails = new LinkageDetails(accountId, linkageKey, nationalPracticeCode);
        }
    }
}
