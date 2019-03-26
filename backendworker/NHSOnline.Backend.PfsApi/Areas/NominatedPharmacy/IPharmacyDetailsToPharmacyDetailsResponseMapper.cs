using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using GeoCoordinatePortable;
using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public interface IPharmacyDetailsToPharmacyDetailsResponseMapper
    {
        PharmacyDetailsResponse Map(Organisation pharmacy);
        
        IEnumerable<PharmacyDetailsResponse> Map(IEnumerable<Organisation> pharmacies, GeoCoordinate postcodeCoordinate);
    }
}
