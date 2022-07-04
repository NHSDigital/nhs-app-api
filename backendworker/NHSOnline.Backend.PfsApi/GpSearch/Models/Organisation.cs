using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.PfsApi.GpSearch.Models
{
    public class Organisation
    {
        public string OrganisationName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string ODSCode { get; set; }

        public Geocode Geocode { get; set; }

        public List<OpeningTime> OpeningTimes { get; set; } = new List<OpeningTime>();

        public List<ContactInformation> Contacts { get; set; } = new List<ContactInformation>();

        public List<MetricInformation> Metrics { get; set; } = new List<MetricInformation>();

        public string OrganisationSubType { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1056",
            Justification = "We want to display the exact URL/string returned from NHSSearch to avoid any parsing error.")]
        public string URL { get; set; }
    }
}
