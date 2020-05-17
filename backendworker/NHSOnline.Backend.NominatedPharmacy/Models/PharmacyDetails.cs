using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class PharmacyDetails
    {
        public string PharmacyName { get; set; }

        public string PharmacyType { get; set; }

        public string PharmacySubType { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1056",
            Justification = "We want to display exact URL string received from NHS-Search to avoid any parsing error.")]
        public string URL { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string TelephoneNumber { get; set; }

        public string OdsCode { get; set; }

        public double? Distance { get; set; }

        public IEnumerable<OpeningTime> OpeningTimes { get; set; } = new List<OpeningTime>();
    }
}