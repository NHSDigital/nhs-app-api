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
                    HouseName = demographics.HouseName,
                    NumberStreet = demographics.RoadName,
                    Village = demographics.Locality,
                    Town = demographics.PostTown,
                    County = demographics.County,
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

        private static string FullAddress(DemographicsData demographicsData)
        {
            var addressParts = new List<string>
            {
                demographicsData.HouseName,
                demographicsData.RoadName,
                demographicsData.Locality,
                demographicsData.PostTown,
                demographicsData.County,
                demographicsData.Postcode
            };

            return string.Join(", ", addressParts.Where(part => !string.IsNullOrEmpty(part)));
        }
    }
}