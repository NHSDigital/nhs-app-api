using NHSOnline.Backend.PfsApi.GpSearch.Models;
using GeoCoordinatePortable;
using System.Collections.Generic;
using NHSOnline.Backend.NominatedPharmacy.Models;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public interface IPharmacyDetailsToPharmacyDetailsResponseMapper
    {
        PharmacyDetails Map(Organisation pharmacy);

        IEnumerable<PharmacyDetails> Map(IEnumerable<Organisation> pharmacies);

        IEnumerable<PharmacyDetails> Map(IEnumerable<Organisation> pharmacies, GeoCoordinate postcodeCoordinate);
    }
}
