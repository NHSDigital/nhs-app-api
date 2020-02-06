using System.Collections.Generic;
using System.Net;
using GeoCoordinatePortable;

namespace NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy
{
    public class PharmacySearchResponse
    {
        public PharmacySearchResponse(
            HttpStatusCode statusCode,
            List<Organisation> pharmacies = null,
            int? pharmacyCount = null)
        {
            StatusCode = statusCode;
            Pharmacies = pharmacies ?? new List<Organisation>();
            PharmacyCount = pharmacyCount;
        }

        public HttpStatusCode StatusCode { get; }
        
        public List<Organisation> Pharmacies { get; }
        
        public int? PharmacyCount { get; }
        
        public GeoCoordinate PostcodeCoordinate { get; set; }
    }
}