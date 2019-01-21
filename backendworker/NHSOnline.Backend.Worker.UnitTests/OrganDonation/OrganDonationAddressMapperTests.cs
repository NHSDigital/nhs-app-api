using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationAddressMapperTests
    {

        private IFixture _fixture;
        private IMapper<string, NHSOnline.Backend.Worker.Areas.OrganDonation.Models.Address, Address>
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
            var address = _fixture.Create<NHSOnline.Backend.Worker.Areas.OrganDonation.Models.Address>();
            
            // Act 
            var result = _organDonationAddressMapper.Map("15 Street, Town, EC1A 1BB", address);

            // Assert
            result.Should().NotBeNull();
            result.Line.Should().ContainSingle().Which.Should().Be(address.Text);
            result.PostalCode.Should().Be(address.PostCode);
        }

        
        [TestMethod]
        [DataRow(null, null)]
        [DataRow("18 Street, Town", null)]
        [DataRow(null, "W1A 0AX")]
        [DataRow("18 Street, Town", "W1A 0AX")]
        public void MapToOrganDonationAddress_WhenPassingSplitAddress_MapAddress(string text, string postcode)
        {
            // Act 
            var result = _organDonationAddressMapper.Map(null,
                new NHSOnline.Backend.Worker.Areas.OrganDonation.Models.Address
                {
                    Text = text,
                    PostCode = postcode
                });

            //Assert
            result.Should().NotBeNull();
            result.Line.Should().ContainSingle().Which.Should().Be(text);
            result.PostalCode.Should().Be(postcode);
        }
    }
}