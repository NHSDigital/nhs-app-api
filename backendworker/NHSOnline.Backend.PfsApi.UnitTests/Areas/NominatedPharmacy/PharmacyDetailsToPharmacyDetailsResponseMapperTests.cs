using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.NominatedPharmacy.Models;
using GeoCoordinatePortable;
using OpeningTime = NHSOnline.Backend.NominatedPharmacy.Models.OpeningTime;
using GpSearchOpeningTime = NHSOnline.Backend.PfsApi.GpSearch.Models.OpeningTime;
using static NHSOnline.Backend.PfsApi.GpSearch.ResponseEnums;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.NominatedPharmacy
{
    [TestClass]
    public class PharmacyDetailsToPharmacyDetailsResponseMapperTests
    {
        private IPharmacyDetailsToPharmacyDetailsResponseMapper _mapper;
        private ILogger<PharmacyDetailsToPharmacyDetailsResponseMapper> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<PharmacyDetailsToPharmacyDetailsResponseMapper>>();
            _mapper = new PharmacyDetailsToPharmacyDetailsResponseMapper(_logger);
        }

        [TestMethod]
        public void MapPharmacyDetailsToPharmacyDetailsResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map((Organisation)null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("pharmacy");
        }

        [TestMethod]
        public void MapPharmacyDetailsToPharmacyDetailsResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            const string phone = "024345322434";

            var pharmacy = new Organisation
            {
                OrganisationName = "Pharmacy 1",
                Address1 = "Bond Steet",
                Address2 = "Blue house",
                Address3 = "Grange meadows",
                City = "London",
                County = "Berkshire",
                Postcode = "RG3 8DJ",
                Contacts = new List<ContactInformation>
                {
                    new ContactInformation{ ContactValue = phone, ContactMethodType = OrganisationContactMethodType.Telephone}
                },
                OpeningTimes = new List<GpSearchOpeningTime>
                {
                    new GpSearchOpeningTime { Weekday = WeekDay.Monday, Times = "01:00-18:00", OpeningTimeType = OpeningTimeType.General, IsOpen = true },
                    new GpSearchOpeningTime { Weekday = WeekDay.Tuesday, Times = "02:00-18:00", OpeningTimeType = OpeningTimeType.General, IsOpen = true },
                    new GpSearchOpeningTime { Weekday = WeekDay.Wednesday, Times = "03:00-18:00", OpeningTimeType = OpeningTimeType.General, IsOpen = true },
                    new GpSearchOpeningTime { Weekday = WeekDay.Thursday, Times = "04:00-18:00", OpeningTimeType = OpeningTimeType.General, IsOpen = true },
                    new GpSearchOpeningTime { Weekday = WeekDay.Friday, Times = "05:00-18:00", OpeningTimeType = OpeningTimeType.General, IsOpen = true },
                    new GpSearchOpeningTime { Weekday = WeekDay.Saturday, Times = "06:00-17:00", OpeningTimeType = OpeningTimeType.General, IsOpen = true },
                    new GpSearchOpeningTime { AdditionalOpeningDate = "Apr 19 2019", IsOpen = false },
                    new GpSearchOpeningTime { AdditionalOpeningDate = "Apr 21 2019", IsOpen = false },
                    new GpSearchOpeningTime { AdditionalOpeningDate = "Apr 22 2019", IsOpen = false },
                    new GpSearchOpeningTime { AdditionalOpeningDate = "May  6 2019", IsOpen = false },
                    new GpSearchOpeningTime { AdditionalOpeningDate = "May 27 2019", IsOpen = false },
                },
            };

            // Act
            var result = _mapper.Map(pharmacy);

            // Assert
            var expectedResult = new PharmacyDetails
            {
                PharmacyName = pharmacy.OrganisationName,
                AddressLine1 = pharmacy.Address1,
                AddressLine2 = pharmacy.Address2,
                AddressLine3 = pharmacy.Address3,
                City = pharmacy.City,
                County = pharmacy.County,
                Postcode = pharmacy.Postcode,
                TelephoneNumber = phone,
                OpeningTimes = new List<OpeningTime>
                {
                    new OpeningTime
                    {
                        Day = "Monday",
                        Time = "01:00-18:00",
                    },
                    new OpeningTime
                    {
                        Day = "Tuesday",
                        Time = "02:00-18:00",
                    },
                    new OpeningTime
                    {
                        Day = "Wednesday",
                        Time = "03:00-18:00",
                    },
                    new OpeningTime
                    {
                        Day = "Thursday",
                        Time = "04:00-18:00",
                    },
                    new OpeningTime
                    {
                        Day = "Friday",
                        Time = "05:00-18:00",
                    },
                    new OpeningTime
                    {
                        Day = "Saturday",
                        Time = "06:00-17:00",
                    },
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapPharmacyDetailsToPharmacyDetailsResponse_WhenCollectionsAreNull_HandlesNullValues()
        {
            // Arrange
            var pharmacy = new Organisation
            {
                OrganisationName = "Pharmacy 1",
                Address1 = "Bond Steet",
                Address2 = "Blue house",
                Address3 = "Grange meadows",
                City = "London",
                County = "Berkshire",
                Postcode = "RG3 8DJ",
                Contacts = null,
                OpeningTimes = null,
                Metrics = null,
            };

            // Act
            var result = _mapper.Map(pharmacy);

            // Assert
            var expectedResult = new PharmacyDetails
            {
                PharmacyName = pharmacy.OrganisationName,
                AddressLine1 = pharmacy.Address1,
                AddressLine2 = pharmacy.Address2,
                AddressLine3 = pharmacy.Address3,
                City = pharmacy.City,
                County = pharmacy.County,
                Postcode = pharmacy.Postcode,
                TelephoneNumber = null,
                OpeningTimes = new List<OpeningTime>()
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapPharmacyDetailsToPharmacyDetailsResponse_WithValuesAndValidGeoCoordinates_ReturnsResultValues()
        {
            // Arrange
            const string phone = "024345322434";
            const double preCalculatedDistanceInMiles = 960.7;

            var pharmacy = new Organisation
            {
                OrganisationName = "Pharmacy 1",
                Address1 = "Bond Steet",
                Address2 = "Blue house",
                Address3 = "Grange meadows",
                City = "London",
                County = "Berkshire",
                Postcode = "RG3 8DJ",
                Geocode = new Geocode
                {
                    Coordinates = new List<double>(new double[] { -20, 20 })
                },
                Contacts = new List<ContactInformation>
                {
                    new ContactInformation{ ContactValue = "1234567890", ContactMethodType = OrganisationContactMethodType.Fax},
                    new ContactInformation{ ContactValue = phone, ContactMethodType = OrganisationContactMethodType.Telephone}
                },
            };

            var postcodeCoordinate = new GeoCoordinate
            {
                Latitude = 10,
                Longitude = -10
            };

            // Act
            var result = _mapper.Map(new List<Organisation> { pharmacy }, postcodeCoordinate);

            // Assert
            var expectedResult = new List<PharmacyDetails>
            {
                new PharmacyDetails
                {
                    PharmacyName = pharmacy.OrganisationName,
                    AddressLine1 = pharmacy.Address1,
                    AddressLine2 = pharmacy.Address2,
                    AddressLine3 = pharmacy.Address3,
                    City = pharmacy.City,
                    County = pharmacy.County,
                    Postcode = pharmacy.Postcode,
                    TelephoneNumber = phone,
                    Distance =  preCalculatedDistanceInMiles,
                },
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapPharmacyDetailsToPharmacyDetailsResponse_WithValuesAndInvalidGeoCoordinates_ReturnsResultValues()
        {
            // Arrange
            const string phone = "024345322434";

            var pharmacy = new Organisation
            {
                OrganisationName = "Pharmacy 1",
                Address1 = "Bond Steet",
                Address2 = "Blue house",
                Address3 = "Grange meadows",
                City = "London",
                County = "Berkshire",
                Postcode = "RG3 8DJ",
                Geocode = new Geocode
                {
                    Coordinates = new List<double>(new double[] { -200, 200 })
                },
                Contacts = new List<ContactInformation>
                {
                    new ContactInformation{ ContactValue = "1234567890", ContactMethodType = OrganisationContactMethodType.Fax},
                    new ContactInformation{ ContactValue = phone, ContactMethodType = OrganisationContactMethodType.Telephone}
                },
            };

            var postcodeCoordinate = new GeoCoordinate
            {
                Latitude = 10,
                Longitude = -10
            };

            // Act
            var result = _mapper.Map(new List<Organisation> { pharmacy }, postcodeCoordinate);

            // Assert
            var expectedResult = new List<PharmacyDetails>
            {
                new PharmacyDetails
                {
                    PharmacyName = pharmacy.OrganisationName,
                    AddressLine1 = pharmacy.Address1,
                    AddressLine2 = pharmacy.Address2,
                    AddressLine3 = pharmacy.Address3,
                    City = pharmacy.City,
                    County = pharmacy.County,
                    Postcode = pharmacy.Postcode,
                    TelephoneNumber = phone,
                    Distance =  null,
                },
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
