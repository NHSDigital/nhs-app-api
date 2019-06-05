using System.Collections.Generic;
namespace NHSOnline.Backend.PfsApi.GpSearch.Models
{
    public class Geocode
    {
        public ResponseEnums.GeoCodeType Type { get; set; }

        public List<double> Coordinates { get; set; }

    }
}
