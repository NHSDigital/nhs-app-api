using System.Collections.Generic;
using GeoCoordinatePortable;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public interface IPharmacyDetailsToPharmacyDetailsResponseMapper
    {
        PharmacyDetails Map(Organisation pharmacy);

        IEnumerable<PharmacyDetails> Map(IEnumerable<Organisation> pharmacies);

        IEnumerable<PharmacyDetails> Map(IEnumerable<Organisation> pharmacies, GeoCoordinate postcodeCoordinate);
    }
}
