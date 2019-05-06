using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models
{
    public class PharmacyDetailsResponse
    {
        public PharmacyDetails PharmacyDetails { get; set; }
        
        public bool NominatedPharmacyEnabled { get; set; }
    }
}
