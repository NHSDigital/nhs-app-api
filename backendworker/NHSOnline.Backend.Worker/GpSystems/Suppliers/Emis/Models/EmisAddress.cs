using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class EmisAddress
    {
        public string HouseNameFlatNumber { get; set; }
        public string NumberStreet { get; set; }
        public string Village { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }

        public override string ToString()
        {
            var addressParts = new List<string>
            {
                HouseNameFlatNumber,
                NumberStreet,
                Village,
                Town,
                County,
                Postcode
            };
            
            return string.Join(", ", addressParts.Where(part => !string.IsNullOrEmpty(part)));
        }
    }
}