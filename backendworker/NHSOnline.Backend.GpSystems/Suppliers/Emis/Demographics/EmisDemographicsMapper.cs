using System;
using System.Linq;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics
{
    public class EmisDemographicsMapper : IEmisDemographicsMapper
    {
        public DemographicsResponse Map(DemographicsGetResponse demographicsGetResponse)
        {
            if (demographicsGetResponse == null)
            {
                throw new ArgumentNullException(nameof(demographicsGetResponse));
            }

            var nhsNumber = string.Empty;
            var allNhsNumbersInResponse = demographicsGetResponse.ExtractNhsNumbers().ToArray();

            if (allNhsNumbersInResponse.Any())
            {
                nhsNumber = allNhsNumbersInResponse.First().NhsNumber;
            }

            return new DemographicsResponse
            {
                PatientName = FormatName(demographicsGetResponse),
                DateOfBirth = demographicsGetResponse.DateOfBirth,
                Sex = demographicsGetResponse.Sex,
                NhsNumber = nhsNumber,
                Address = demographicsGetResponse.Address?.ToString(),
                AddressParts = new DemographicsAddress
                {
                    Text = demographicsGetResponse.Address?.ToString(AddressExclusion.Postcode),
                    Postcode = demographicsGetResponse.Address?.Postcode
                },
                NameParts = new DemographicsName
                {
                    Title = demographicsGetResponse.Title,
                    Given = demographicsGetResponse.FirstName,
                    Surname = demographicsGetResponse.Surname
                }
            };
        }

        private static string FormatName(DemographicsGetResponse demographicsGetResponse)
        {
            return string.Join(" ",
                new[]
                    {
                        demographicsGetResponse.Title, demographicsGetResponse.FirstName,
                        demographicsGetResponse.Surname
                    }
                    .Where(part => !string.IsNullOrEmpty(part)));
        }
    }
}