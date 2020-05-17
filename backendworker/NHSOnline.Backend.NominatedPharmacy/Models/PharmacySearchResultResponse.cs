using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.NominatedPharmacy.Models
{
    public class PharmacySearchResultResponse
    {
        public IEnumerable<PharmacyDetails> Pharmacies { get; set; } = Enumerable.Empty<PharmacyDetails>();

        public int? PharmacyCount { get; set; }
    }
}
