using System;
using System.Globalization;
using System.Linq;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Demographics
{
    public class VisionDemographicsMapper : IVisionDemographicsMapper
    {
        private static readonly TextInfo UkCultureTextInfo = new CultureInfo("en-GB", false).TextInfo;

        public DemographicsResponse Map(VisionDemographics patientDemographics, string nhsNumber)
        {
            if (patientDemographics == null)
            {
                throw new ArgumentNullException(nameof(patientDemographics));
            }

            if (string.IsNullOrEmpty(nhsNumber))
            {
                throw new ArgumentNullException(nameof(nhsNumber));
            }

            var formattedAddress = FormatAddress(patientDemographics.PrimaryAddress);

            return new DemographicsResponse
            {
                PatientName = FormatName(patientDemographics),
                DateOfBirth = patientDemographics.DateOfBirth,
                Sex = patientDemographics.Gender?.Text,
                NhsNumber = nhsNumber,
                Address = formattedAddress?.ToString(),
                AddressParts = new DemographicsAddress
                {
                    HouseName = formattedAddress?.HouseName,
                    NumberStreet = formattedAddress?.HouseNumberStreet,
                    Town = formattedAddress?.Town,
                    County = formattedAddress?.County,
                    Postcode = formattedAddress?.Postcode
                },
                NameParts = new DemographicsName
                {
                    Title = patientDemographics.Name?.Title,
                    Given = patientDemographics.Name?.Forename,
                    Surname = patientDemographics.Name?.Surname
                }
            };
        }

        private static string FormatName(VisionDemographics patientDemographics)
        {
            return patientDemographics.Name != null
                ? JoinNotNull(" ", ToTitleCase(patientDemographics.Name.Title),
                    ToTitleCase(patientDemographics.Name.Forename), ToTitleCase(patientDemographics.Name.Surname))
                : string.Empty;
        }

        private static string JoinNotNull(string delimiter, params string[] parts) => string.Join(delimiter, parts
            .Where(part => !string.IsNullOrEmpty(part)));

        private static FormattedAddress FormatAddress(PrimaryAddress primaryAddress)
        {
            if (primaryAddress == null)
            {
                return null;
            }

            var houseNumberStreet = FormatHouseNumberAndStreet(primaryAddress.HouseNumber, primaryAddress.Street);

            return new FormattedAddress
            {
                HouseName = ToTitleCase(primaryAddress.HouseName),
                HouseNumberStreet = ToTitleCase(houseNumberStreet),
                Town = ToTitleCase(primaryAddress.Town),
                County = ToTitleCase(primaryAddress.County),
                Postcode = primaryAddress.Postcode
            };
        }

        private static string ToTitleCase(string text)
        {
            return !string.IsNullOrEmpty(text)
                ? UkCultureTextInfo.ToTitleCase(UkCultureTextInfo.ToLower(text))
                : null;
        }

        private static string FormatHouseNumberAndStreet(string houseNumber, string street)
        {
            return !string.IsNullOrEmpty(houseNumber)
                ? !string.IsNullOrEmpty(street) ? $"{houseNumber} {street}" : string.Empty
                : street;
        }

        private class FormattedAddress
        {
            public string HouseName { get; set; }

            public string HouseNumberStreet { get; set; }

            public string Town { get; set; }

            public string County { get; set; }

            public string Postcode { get; set; }

            public override string ToString() => JoinNotNull(", ",  HouseName, HouseNumberStreet,
                Town, County, Postcode);
        }
    }
}