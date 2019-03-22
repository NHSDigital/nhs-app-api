using System.Collections.Generic;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.Worker.Areas.NominatedPharmacy.Models
{
    public class PharmaciesSearchResponse
    {
        public PharmaciesSearchResponse(List<Organisation> pharmacies = null)
        {
            this.Pharmacies = pharmacies ?? new List<Organisation>();
        }

        public List<Organisation> Pharmacies { get; }
    }
}
