using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using GeoCoordinatePortable;
using NHSOnline.Backend.Worker.GpSearch.Models;
using OpeningTime = NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy.Models.OpeningTime;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.NominatedPharmacy
{
    [TestClass]
    public class PharmacyDetailsToPharmacyDetailsResponseMapperTests
    {
        private IFixture _fixture;
        private IPharmacyDetailsToPharmacyDetailsResponseMapper _mapper;
        private ILogger<PharmacyDetailsToPharmacyDetailsResponseMapper> _logger;
        
        
        [TestInitialize]
        public void TestInitialize()
        {
            _logger = Mock.Of<ILogger<PharmacyDetailsToPharmacyDetailsResponseMapper>>();
            _mapper = new PharmacyDetailsToPharmacyDetailsResponseMapper(_logger);

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void MapPharmacyDetailsToPharmacyDetailsResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map(null);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("pharmacy");
        }

        [TestMethod]
        public void MapPharmacyDetailsToPharmacyDetailsResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var phone = "024345322434";

            var pharmacy = new Organisation
            {
                OrganisationName = "Pharmacy 1",
                Address1 = "Bond Steet",
                Address2 = "Blue house",
                Address3 = "Grange meadows",
                City = "London",
                County = "Berkshire",
                Postcode = "RG3 8DJ",
                Contacts = "[{\"OrganisationContactType\":\"Primary\",\"OrganisationContactAvailabilityType\":\"Office hours\",\"OrganisationContactMethodType\":\"Telephone\",\"OrganisationContactValue\":\"" + phone + "\"}]",
                OpeningTimes =
                "[{\"WeekDay\":\"Monday\",\"Times\":\"01:00-18:00\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"\",\"IsOpen\":true}," +
                "{\"WeekDay\":\"Tuesday\",\"Times\":\"02:00-18:00\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"\",\"IsOpen\":true}," +
                "{\"WeekDay\":\"Wednesday\",\"Times\":\"03:00-18:00\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"\",\"IsOpen\":true}," +
                "{\"WeekDay\":\"Thursday\",\"Times\":\"04:00-18:00\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"\",\"IsOpen\":true}," +
                "{\"WeekDay\":\"Friday\",\"Times\":\"05:00-18:00\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"\",\"IsOpen\":true}," +
                "{\"WeekDay\":\"Saturday\",\"Times\":\"06:00-17:00\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"\",\"IsOpen\":true}," +
                "{\"WeekDay\":\"\",\"Times\":\"\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"Apr 19 2019\",\"IsOpen\":false}," +
                "{\"WeekDay\":\"\",\"Times\":\"\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"Apr 21 2019\",\"IsOpen\":false}," +
                "{\"WeekDay\":\"\",\"Times\":\"\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"Apr 22 2019\",\"IsOpen\":false}," +
                "{\"WeekDay\":\"\",\"Times\":\"\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"May  6 2019\",\"IsOpen\":false}," +
                "{\"WeekDay\":\"\",\"Times\":\"\",\"OpeningTimeType\":\"General\",\"AdditionalOpeningDate\":\"May 27 2019\",\"IsOpen\":false}]",
            };
            
            // Act
            var result = _mapper.Map(pharmacy);

            // Assert
            result.Should().NotBeNull();

            var expectedResult = new PharmacyDetailsResponse
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
        public void MapPharmacyDetailsToPharmacyDetailsResponse_WithValuesAndValidGeoCoordinates_ReturnsResultValues()
        {
            // Arrange
            var phone = "024345322434";
            var preCalculatedDistanceInMiles = 960.7;
            
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
                Contacts = "[{\"OrganisationContactType\":\"Primary\",\"OrganisationContactAvailabilityType\":\"Office hours\",\"OrganisationContactMethodType\":\"Fax\",\"OrganisationContactValue\":\"1234567890\"}," +
                           "{\"OrganisationContactType\":\"Primary\",\"OrganisationContactAvailabilityType\":\"Office hours\",\"OrganisationContactMethodType\":\"Telephone\",\"OrganisationContactValue\":\"" + phone + "\"}]",
            };
            
            var postcodeCoordinate = new GeoCoordinate
            {
                Latitude = 10,
                Longitude = -10               
            };
                        
            // Act
            var result = _mapper.Map(new List<Organisation> { pharmacy }, postcodeCoordinate);

            // Assert
            result.Should().NotBeNull();

            var expectedResult = new List<PharmacyDetailsResponse>
            {
                new PharmacyDetailsResponse
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
            var phone = "024345322434";

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
                Contacts = "[{\"OrganisationContactType\":\"Primary\",\"OrganisationContactAvailabilityType\":\"Office hours\",\"OrganisationContactMethodType\":\"Fax\",\"OrganisationContactValue\":\"1234567890\"}," +
                           "{\"OrganisationContactType\":\"Primary\",\"OrganisationContactAvailabilityType\":\"Office hours\",\"OrganisationContactMethodType\":\"Telephone\",\"OrganisationContactValue\":\"" + phone + "\"}]",
            };

            var postcodeCoordinate = new GeoCoordinate
            {
                Latitude = 10,
                Longitude = -10               
            };
                        
            // Act
            var result = _mapper.Map(new List<Organisation> { pharmacy }, postcodeCoordinate);

            // Assert
            result.Should().NotBeNull();

            var expectedResult = new List<PharmacyDetailsResponse>
            {
                new PharmacyDetailsResponse
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
