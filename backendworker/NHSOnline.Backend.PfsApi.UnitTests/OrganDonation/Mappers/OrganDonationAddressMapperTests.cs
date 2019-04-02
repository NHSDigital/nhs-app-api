using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Mappers;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation.Mappers
{
    [TestClass]
    public class OrganDonationAddressMapperTests
    {

        private IFixture _fixture;
        private IMapper<string,NHSOnline.Backend.PfsApi.OrganDonation.Models.Address, Address>
            _organDonationAddressMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _organDonationAddressMapper = _fixture.Create<OrganDonationAddressMapper>();
        }

        [TestMethod]
        public void MapToOrganDonationAddress_WhenPassingNoFullAddressAndNoSplitAddress_ThrowsAnException()
        {
            // Act and Assert
            Action act = () => _organDonationAddressMapper.Map(null, null);

            act.Should().Throw<ArgumentException>();
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("           ")]
        public void MapToOrganDonationAddress_WhenPassingEmptyFullAddressAndNoSplitAddress_ThrowsAnException(
            string fullAddress)
        {
            // Act and Assert
            Action act = () => _organDonationAddressMapper.Map(fullAddress, null);

            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        [DataRow("15 Street, Town, EC1A 1BB", "EC1A 1BB")]
        [DataRow("15 Street, Town, W1A 0AX", "W1A 0AX")]
        [DataRow("15 Street, Town, M1 1AE", "M1 1AE")]
        [DataRow("15 Street, Town, B33 8TH", "B33 8TH")]
        [DataRow("15 Street, Town, CR2 6XH", "CR2 6XH")]
        [DataRow("15 Street, Town, DN55 1PT", "DN55 1PT")]
        public void MapToOrganDonationAddress_WhenPassingFullAddress_With_Common_Postcodes_ParsesFullAddress(
            string fullAddress, string mappedPostcode)
        {
            // Act 
            var result = _organDonationAddressMapper.Map(fullAddress, null);

            // Assert
            result.Should().NotBeNull();
            result.Line.Should().ContainSingle().Which.Should().Be("15 Street, Town,");
            result.PostalCode.Should().Be(mappedPostcode);
        }

        [TestMethod]
        [DataRow("15 Street, Town, W1A 0AX, EC1A 1BB", "15 Street, Town, W1A 0AX,", "EC1A 1BB")]
        [DataRow("15 Street, EC1A 1BB, Town, W1A 0AX", "15 Street, EC1A 1BB, Town,", "W1A 0AX")]
        public void MapToOrganDonationAddress_WhenPassingFullAddress_With_Two_Postcodes_Should_Pick_The_Last_One(
            string fullAddress, string mappedLine, string mappedPostcode)
        {
            // Act 
            var result = _organDonationAddressMapper.Map(fullAddress, null);

            // Assert
            result.Should().NotBeNull();
            result.Line.Should().ContainSingle().Which.Should().Be(mappedLine);
            result.PostalCode.Should().Be(mappedPostcode);
        }

        [TestMethod]
        [DataRow("15 Street, Town")]
        [DataRow("15 Street, Town, W1A 0A")]
        public void MapToOrganDonationAddress_WhenPassingFullAddress_With_InvalidPostcode_ParsesAddress(
            string fullAddress)
        {
            // Act 
            var result = _organDonationAddressMapper.Map(fullAddress, null);

            //Assert
            result.Should().NotBeNull();
            result.Line.Should().ContainSingle().Which.Should().Be(fullAddress);
            result.PostalCode.Should().BeNull();
        }
        
        [TestMethod]
        public void MapToOrganDonationAddress_WhenPassingBothAddresss_UseSplitAddress()
        {
            // Arrange
            var address = new NHSOnline.Backend.PfsApi.OrganDonation.Models.Address
            {
                HouseName = "House name Split",
                NumberStreet = "Street Split",
                Village = "Village Split",
                Town = "Town Split",
                County = "County Split",
                PostCode = "EC1A 1BB"
            };
            
            // Act 
            var result = _organDonationAddressMapper.Map("15 Street, Town, EC1A 1BB", address);

            // Assert
            result.Should().NotBeNull();
            result.Line.Should().ContainInOrder(address.HouseName, address.NumberStreet, address.Village,
                $"{address.Town}, {address.County}");
            result.PostalCode.Should().Be(address.PostCode);
        }

        
        [TestMethod]
        [DataRow("house name", "street", "village", "town", "county", "W1A 0AX", "house name", "street", "village", "town, county")]
        [DataRow(null, "15 street", "village", "town", "county", "W1A 0AX", "15 street", "village", "town", "county")]
        [DataRow(null, "15 street", null, "town", "county", "W1A 0AX", "15 street", "town", "county", null)]
        [DataRow(null, "15 street", null, "town", null, "W1A 0AX", "15 street", "town", null, null)]
        [DataRow("house name", null, null, null, null, "W1A 0AX", "house name", null, null, null)]
        public void MapToOrganDonationAddress_WhenPassingSplitAddress_MapAddress(
            string houseName,
            string numberStreet,
            string village,
            string town,
            string county,
            string postcode,
            string firstLine,
            string secondLine,
            string thirdLine,
            string fourthLine)
        {
            // Arrange
            var expected = new[] { firstLine, secondLine, thirdLine, fourthLine }
                .Where(s => !string.IsNullOrEmpty(s));
            
            // Act 
            var result = _organDonationAddressMapper.Map(null,
                new PfsApi.OrganDonation.Models.Address
                {
                    HouseName = houseName,
                    NumberStreet = numberStreet,
                    Village = village,
                    Town = town,
                    County = county,
                    PostCode = postcode
                });

            //Assert
            result.Should().NotBeNull();
            result.Line.Should().ContainInOrder(expected);
            result.PostalCode.Should().Be(postcode);
        }
    }
}