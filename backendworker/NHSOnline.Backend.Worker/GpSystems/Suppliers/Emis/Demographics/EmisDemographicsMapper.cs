using System;
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
                PatientName = FormatName(demographicsGetResponse),
                DateOfBirth = demographicsGetResponse.DateOfBirth,
                Sex = demographicsGetResponse.Sex,
                NhsNumber = demographicsGetResponse.ExtractNhsNumbers().Any() ? demographicsGetResponse.ExtractNhsNumbers().First().NhsNumber : null,
                Address = demographicsGetResponse.Address?.ToString()
            };
        }

        private static string FormatName(DemographicsGetResponse demographicsGetResponse)
        {
            return string.Join(" ", new[] { demographicsGetResponse.Title, demographicsGetResponse.FirstName, demographicsGetResponse.Surname }
                .Where(part => !string.IsNullOrEmpty(part)));
        }
    }
}
