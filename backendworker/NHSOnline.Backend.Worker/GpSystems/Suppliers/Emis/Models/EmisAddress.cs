using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Demographics;

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
            return ToString(AddressExclusion.None);
        }
        
        public string ToString(AddressExclusion exclusion)
        {
            var addressParts = new List<string>
            {
                HouseNameFlatNumber,
                NumberStreet,
                Village,
                Town,
                County
            };
            
            if (exclusion != AddressExclusion.Postcode)
                addressParts.Add(Postcode);
            
            return string.Join(", ", addressParts.Where(part => !string.IsNullOrEmpty(part)));
        }
    }
}