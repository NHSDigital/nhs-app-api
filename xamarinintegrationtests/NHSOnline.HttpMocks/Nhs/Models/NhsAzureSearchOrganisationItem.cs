using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.HttpMocks.Nhs.Models
{
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only",
        Justification = "Required for mocks")]
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