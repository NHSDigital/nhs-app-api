using System;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics
{
    public class EmisDemographicsMapper : IEmisDemographicsMapper
    {
        public DemographicsResponse Map(DemographicsGetResponse demographicsGetResponse)
        {
            if (demographicsGetResponse == null)
            {
                throw new ArgumentNullException(nameof(demographicsGetResponse));
            }
            
            return new DemographicsResponse
            {
                FirstName = demographicsGetResponse.FirstName,
                Surname = demographicsGetResponse.Surname,
                DateOfBirth = demographicsGetResponse.DateOfBirth,
                Sex = demographicsGetResponse.Sex,
                NhsNumber = demographicsGetResponse.ExtractNhsNumbers().Any() ? demographicsGetResponse.ExtractNhsNumbers().First().NhsNumber : null,
                Address = new Address
                {
                    Line1 = demographicsGetResponse.Address?.HouseNameFlatNumber,
                    Line2 = demographicsGetResponse.Address?.NumberStreet,
                    Line3 = demographicsGetResponse.Address?.Village,
                    Town = demographicsGetResponse.Address?.Town,
                    County = demographicsGetResponse.Address?.County,
                    Postcode = demographicsGetResponse.Address?.Postcode
                }
            };
        }
    }
}
