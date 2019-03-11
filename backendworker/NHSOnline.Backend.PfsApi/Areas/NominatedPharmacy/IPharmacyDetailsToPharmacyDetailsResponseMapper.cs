using NHSOnline.Backend.Worker.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.Worker.Areas.NominatedPharmacy
{
    public interface IPharmacyDetailsToPharmacyDetailsResponseMapper
    {
        PharmacyDetailsResponse Map(Organisation pharmacy);
    }
}
