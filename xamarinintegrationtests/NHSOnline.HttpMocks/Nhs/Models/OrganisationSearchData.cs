namespace NHSOnline.HttpMocks.Nhs.Models
{
    public class OrganisationSearchData
    {
        public string? Select { get; set; }
        public string? Filter { get; set; }
        public bool Count { get; set; }
        public long Top { get; set; }
        public string? Search { get; set; }
    }
}