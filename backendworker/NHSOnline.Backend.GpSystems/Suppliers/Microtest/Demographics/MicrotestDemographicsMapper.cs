using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics
{
    public class MicrotestDemographicsMapper : IMicrotestDemographicsMapper
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
                DateOfBirth = demographicsGetResponse.Dob,
                Sex = demographicsGetResponse.Sex,
                NhsNumber = demographicsGetResponse.Nhs,
                Address = FullAddress(demographicsGetResponse),
                AddressParts = new DemographicsAddress
                {
                    Text = FullAddress(demographicsGetResponse, AddressExclusion.Postcode),
                    Postcode = demographicsGetResponse.Postcode
                },
                NameParts = new DemographicsName
                {
                    Title = demographicsGetResponse.Title,
                    Given = demographicsGetResponse.Forenames1,
                    Surname = demographicsGetResponse.Surname
                }
            };
        }

        private static string FormatName(DemographicsGetResponse demographicsGetResponse)
        {
            return string.Join(" ",
                new[]
                    {
                        demographicsGetResponse.Title,
                        demographicsGetResponse.Forenames1,
                        demographicsGetResponse.Forenames2,
                        demographicsGetResponse.Surname
                    }
                    .Where(part => !string.IsNullOrEmpty(part)));
        }

        private static string FullAddress(DemographicsGetResponse demographicsGetResponse, AddressExclusion? exclusion = null)
        {
            var addressParts = new List<string>
            {
                demographicsGetResponse.HouseName,
                demographicsGetResponse.RoadName,
                demographicsGetResponse.Locality,
                demographicsGetResponse.PostTown,
                demographicsGetResponse.County
            };

            if (exclusion != AddressExclusion.Postcode)
                addressParts.Add(demographicsGetResponse.Postcode);

            return string.Join(", ", addressParts.Where(part => !string.IsNullOrEmpty(part)));
        }
    }
}