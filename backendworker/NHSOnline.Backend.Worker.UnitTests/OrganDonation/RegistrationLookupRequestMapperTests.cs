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
    public class RegistrationLookupRequestMapperTests
    {
        private IFixture _fixture;
        private IMapper<OrganDonationRegistration, RegistrationLookupRequest> _registrationToLookupRequestMapper;


        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _registrationToLookupRequestMapper = _fixture.Create<RegistrationLookupRequestMapper>();
        }

        [TestMethod]
        public void MapOrganDonationRegistrationToLookupRequest_WhenPassingNull_ThrowsArgumentNullException()
        {
            // Act and Assert
            Action act = () => _registrationToLookupRequestMapper.Map(null);


            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void MapOrganDonationRegistrationToLookupRequest_WithAllValues_MapsCorrectly()
        {
            // Arrange
            var registration = _fixture.Create<OrganDonationRegistration>();
            registration.DateOfBirth = new DateTime(1992, 10, 22);

            // Act
            var result = _registrationToLookupRequestMapper.Map(registration);

            // Assert
            result.Should().NotBeNull();
            result.Given.Should().Be(registration.Name.GivenName);
            result.Family.Should().Be(registration.Name.Surname);
            result.BirthDate.Should().Be("1992-10-22");
            result.NhsNumber.Should().Be(registration.NhsNumber);
            result.Identifier.Should().Be($"https://fhir.nhs.uk/Id/nhs-number|{registration.NhsNumber}");
        }
        
        [TestMethod]
        public void MapOrganDonationRegistrationToLookupRequest_WithNoName_MapsCorrectly()
        {
            // Arrange
            var registration = _fixture.Create<OrganDonationRegistration>();
            registration.Name = null;

            // Act
            var result = _registrationToLookupRequestMapper.Map(registration);

            // Assert
            result.Should().NotBeNull();
            result.Given.Should().Be(string.Empty);
            result.Family.Should().Be(string.Empty);
        }
        
        [TestMethod]
        public void MapOrganDonationRegistrationToLookupRequest_WithSpacesOnNhsNumber_MapsCorrectly()
        {
            // Arrange
            var registration = new OrganDonationRegistration
            {
                NhsNumber = "Number with spaces"
            };

            // Act
            var result = _registrationToLookupRequestMapper.Map(registration);

            // Assert
            result.Should().NotBeNull();
            result.NhsNumber.Should().Be("Numberwithspaces");
            result.Identifier.Should().Be("https://fhir.nhs.uk/Id/nhs-number|Numberwithspaces");
        }
    }
}