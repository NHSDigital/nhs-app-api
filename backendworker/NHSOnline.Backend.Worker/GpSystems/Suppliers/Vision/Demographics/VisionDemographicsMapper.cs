using System;
using System.Globalization;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Demographics.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Demographics
{
    public class VisionDemographicsMapper: IVisionDemographicsMapper
    {
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
            
            return new DemographicsResponse
            {
                PatientName = FormatName(patientDemographics),
                DateOfBirth = patientDemographics.DateOfBirth,
                Sex = patientDemographics.Gender?.Text,
                NhsNumber = nhsNumber,
                Address = FormatAddress(patientDemographics.PrimaryAddress),
            };
        }
        
        private static string FormatName(VisionDemographics patientDemographics)
        {
            var cultureInfo = new CultureInfo("en-GB", false).TextInfo;
            
            return patientDemographics.Name != null ?  
                string.Join(" ", new[]
                    {
                        !string.IsNullOrEmpty(patientDemographics.Name.Title) ?
                            cultureInfo.ToTitleCase(cultureInfo.ToLower(patientDemographics.Name.Title)) : string.Empty,
                        !string.IsNullOrEmpty(patientDemographics.Name.Forename) ?
                            cultureInfo.ToTitleCase(cultureInfo.ToLower(patientDemographics.Name.Forename)) : string.Empty,
                        !string.IsNullOrEmpty(patientDemographics.Name.Surname) ?
                            cultureInfo.ToTitleCase(cultureInfo.ToLower(patientDemographics.Name.Surname)) : string.Empty,
                    }
                    .Where(part => !string.IsNullOrEmpty(part))) : 
                string.Empty;
        }
        
        private static string FormatAddress(PrimaryAddress primaryAddress)
        {
            if (primaryAddress == null)
                return string.Empty;
            
            var cultureInfo = new CultureInfo("en-GB", false).TextInfo;

            var houseNumberStreet = FormatHouseNumberAndStreet(primaryAddress.HouseNumber, primaryAddress.Street);

            return
                string.Join(", ", new[]
                    {
                        !string.IsNullOrEmpty(primaryAddress.HouseName)
                            ? cultureInfo.ToTitleCase(cultureInfo.ToLower(primaryAddress.HouseName))
                            : string.Empty,
                        !string.IsNullOrEmpty(houseNumberStreet)
                            ? cultureInfo.ToTitleCase(cultureInfo.ToLower(houseNumberStreet))
                            : string.Empty,
                        !string.IsNullOrEmpty(primaryAddress.Town)
                            ? cultureInfo.ToTitleCase(cultureInfo.ToLower(primaryAddress.Town))
                            : string.Empty,
                        !string.IsNullOrEmpty(primaryAddress.County)
                            ? cultureInfo.ToTitleCase(cultureInfo.ToLower(primaryAddress.County))
                            : string.Empty,
                        primaryAddress.Postcode
                    }
                    .Where(part => !string.IsNullOrEmpty(part)));
        }

        private static string FormatHouseNumberAndStreet(string housenumber, string street)
        {
            return !string.IsNullOrEmpty(housenumber)
                ? !string.IsNullOrEmpty(street) ? string.Format(CultureInfo.InvariantCulture, "{0} {1}", housenumber, street) :
                string.Empty
                : street;
        }
    }
}