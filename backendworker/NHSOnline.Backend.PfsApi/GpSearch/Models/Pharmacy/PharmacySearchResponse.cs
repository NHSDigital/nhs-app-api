using System.Collections.Generic;
using System.Net;
using GeoCoordinatePortable;

namespace NHSOnline.Backend.PfsApi.GpSearch.Models.Pharmacy
{
    public class PharmacySearchResponse
    {
        public PharmacySearchResponse(HttpStatusCode statusCode, List<Organisation> pharmacies = null)
        {
            StatusCode = statusCode;
            Pharmacies = pharmacies ?? new List<Organisation>();
        }

        public HttpStatusCode StatusCode { get; }
        
        public List<Organisation> Pharmacies { get; }
        
        public GeoCoordinate PostcodeCoordinate { get; set; }
    }
}