using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models
{
    public class PharmacyDetailsResponse
    {
        public string PharmacyName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string TelephoneNumber { get; set; }

        public IEnumerable<OpeningTime> OpeningTimes { get; set; } = new List<OpeningTime>();
    }
}
