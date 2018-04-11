namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class LinkageDetails
    {
        public string AccountId { get; set; }
        public string LinkageKey { get; set; }
        public string NationalPracticeCode { get; set; }

        public LinkageDetails(string accountId, string linkageKey, string nationalPracticeCode)
        {
            AccountId = accountId;
            LinkageKey = linkageKey;
            NationalPracticeCode = nationalPracticeCode;
        }
    }
}
