using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Demographics;
using NHSOnline.Backend.Support;

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

            if (demographicsGetResponse.Demographics == null)
            {
                throw new ArgumentException("demographics object is null", nameof(demographicsGetResponse));
            }

            var demographics = demographicsGetResponse.Demographics;

            return new DemographicsResponse
            {
                PatientName = FormatName(demographics),
                DateOfBirth = demographics.Dob,
                Sex = demographics.Sex,
                NhsNumber = demographics.Nhs.FormatToNhsNumber(),
                Address = FullAddress(demographics),
                AddressParts = new DemographicsAddress
                {
                    Text = FullAddress(demographics, AddressExclusion.Postcode),
                    Postcode = demographics.Postcode
                },
                NameParts = new DemographicsName
                {
                    Title = demographics.Title,
                    Given = demographics.Forenames1,
                    Surname = demographics.Surname
                }
            };
        }

        private static string FormatName(DemographicsData demographicsData)
        {
            return string.Join(" ",
                new[]
                    {
                        demographicsData.Title,
                        demographicsData.Forenames1,
                        demographicsData.Forenames2,
                        demographicsData.Surname
                    }
                    .Where(part => !string.IsNullOrEmpty(part)));
        }

        private static string FullAddress(DemographicsData demographicsData, AddressExclusion? exclusion = null)
        {
            var addressParts = new List<string>
            {
                demographicsData.HouseName,
                demographicsData.RoadName,
                demographicsData.Locality,
                demographicsData.PostTown,
                demographicsData.County
            };

            if (exclusion != AddressExclusion.Postcode)
                addressParts.Add(demographicsData.Postcode);

            return string.Join(", ", addressParts.Where(part => !string.IsNullOrEmpty(part)));
        }
    }
}