using System.Collections.Generic;

namespace NHSOnline.HttpMocks.Nhs.Models
{
    public class NhsAzureSearchOrganisationItem
    {
        public string? OrganisationName{ get; set; }
        public string? OrganisationType { get; set; }
        public string? OrganisationSubType { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? City { get; set; }
        public string? County { get; set; }
        public string? Postcode { get; set; }
        public string? ODSCode { get; set; }
        public IList<Metric>? Metrics { get; set; }
    }
}