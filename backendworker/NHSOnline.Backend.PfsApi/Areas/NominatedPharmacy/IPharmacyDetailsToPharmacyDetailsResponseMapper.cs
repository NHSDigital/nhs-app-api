using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public interface IPharmacyDetailsToPharmacyDetailsResponseMapper
    {
        PharmacyDetailsResponse Map(Organisation pharmacy);
    }
}
