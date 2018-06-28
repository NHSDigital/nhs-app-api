using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Demographics.Models;
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
                PatientName = String.Format("{0} {1}", demographicsGetResponse.FirstName, demographicsGetResponse.Surname),
                DateOfBirth = demographicsGetResponse.DateOfBirth,
                Sex = demographicsGetResponse.Sex,
                NhsNumber = demographicsGetResponse.ExtractNhsNumbers().Any() ? demographicsGetResponse.ExtractNhsNumbers().First().NhsNumber : null,
                Address = demographicsGetResponse.Address?.ToString()
            };
        }
    }
}
