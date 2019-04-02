using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Mappers;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using Name = NHSOnline.Backend.PfsApi.OrganDonation.Models.Name;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation.Mappers
{
    [TestClass]
    public class OrganDonationRegistrationMapperTests
    {
        private IFixture _fixture;
        private IMapper<DemographicsResponse, OrganDonationRegistration> _demographicsToRegistrationMapper;

        private IMapper<OrganDonationRegistration, RegistrationLookupResponse, OrganDonationRegistration>
            _lookupToRegistrationMapper;

        private Mock<IEnumMapper<string, Decision>> _decisionMapper;
        private Mock<IEnumMapper<string, FaithDeclaration>> _faithDeclarationMapper;
        private Mock<IEnumMapper<string, ChoiceState>> _choiceStateMapper;
        private Mock<IMapper<string, DemographicsName, Name>> _demographicsNameMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            
            _decisionMapper = _fixture.Freeze<Mock<IEnumMapper<string, Decision>>>();
            _faithDeclarationMapper = _fixture.Freeze<Mock<IEnumMapper<string, FaithDeclaration>>>();
            _choiceStateMapper = _fixture.Freeze<Mock<IEnumMapper<string, ChoiceState>>>();
            _demographicsNameMapper = _fixture.Freeze<Mock<IMapper<string, DemographicsName, Name>>>();

            _demographicsToRegistrationMapper = _fixture.Create<OrganDonationRegistrationMapper>();
            _lookupToRegistrationMapper = _fixture.Create<OrganDonationRegistrationMapper>();
        }

        [TestMethod]
        public void MapDemographicsResponseToOrganDonationRegistration_WhenPassingNull_ThrowsArgumentNullException()
        {
            // Act and Assert
            Action act = () => _demographicsToRegistrationMapper.Map(null);


            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void MapDemographicsResponseToOrganDonationRegistration_WithAllValues_MapsCorrectly()
        {
            // Arrange
            var response = _fixture.Create<DemographicsResponse>();
            _demographicsNameMapper
                .Setup(x => x.Map(response.PatientName, response.NameParts))
                .Returns(new Name
                {
                    GivenName = response.NameParts.Given,
                    Surname = response.NameParts.Surname,
                    Title = response.NameParts.Title
                })
                .Verifiable();

            // Act
            var result = _demographicsToRegistrationMapper.Map(response);

            // Assert
            _demographicsNameMapper.Verify();
            result.Should().NotBeNull();
            result.NameFull.Should().Be(response.PatientName);
            result.Name.Should().NotBeNull();
            result.Name.GivenName.Should().Be(response.NameParts.Given);
            result.Name.Surname.Should().Be(response.NameParts.Surname);
            result.Name.Title.Should().Be(response.NameParts.Title);
            result.AddressFull.Should().Be(response.Address);
            result.Address.Should().NotBeNull();
            result.Address.HouseName.Should().Be(response.AddressParts.HouseName);
            result.Address.NumberStreet.Should().Be(response.AddressParts.NumberStreet);
            result.Address.Village.Should().Be(response.AddressParts.Village);
            result.Address.Town.Should().Be(response.AddressParts.Town);
            result.Address.County.Should().Be(response.AddressParts.County);
            result.Address.PostCode.Should().Be(response.AddressParts.Postcode);
            result.DateOfBirth.Should().Be(response.DateOfBirth);
            result.NhsNumber.Should().Be(response.NhsNumber);
            result.Gender.Should().Be(response.Sex);
        }

        [TestMethod]
        public void MapDemographicsResponseToOrganDonationRegistration_WithNoAddressParts_MapsCorrectly()
        {
            // Arrange
            var response = _fixture.Create<DemographicsResponse>();
            response.AddressParts = null;
            _demographicsNameMapper
                .Setup(x => x.Map(response.PatientName, response.NameParts))
                .Returns(new Name
                {
                    GivenName = response.NameParts.Given,
                    Surname = response.NameParts.Surname,
                    Title = response.NameParts.Title
                })
                .Verifiable();

            // Act
            var result = _demographicsToRegistrationMapper.Map(response);

            // Assert
            _demographicsNameMapper.Verify();
            result.Should().NotBeNull();
            result.NameFull.Should().Be(response.PatientName);
            result.Name.Should().NotBeNull();
            result.Name.GivenName.Should().Be(response.NameParts.Given);
            result.Name.Surname.Should().Be(response.NameParts.Surname);
            result.Name.Title.Should().Be(response.NameParts.Title);
            result.AddressFull.Should().Be(response.Address);
            result.Address.Should().BeNull();
            result.DateOfBirth.Should().Be(response.DateOfBirth);
            result.NhsNumber.Should().Be(response.NhsNumber);
            result.Gender.Should().Be(response.Sex);
        }

        [TestMethod]
        public void
            MapRegistrationLookupResponseToOrganDonationRegistration_WhenPassingNullRegistration_ThrowsArgumentNullException()
        {
            // Arrange
            var response = _fixture.Create<RegistrationLookupResponse>();

            // Act and Assert
            Action act = () => _lookupToRegistrationMapper.Map(null, response);


            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("firstSource");
        }

        [TestMethod]
        public void
            MapRegistrationLookupResponseToOrganDonationRegistration_WhenPassingNullResponse_ThrowsArgumentNullException()
        {
            // Arrange
            var registration = _fixture.Create<OrganDonationRegistration>();

            // Act and Assert
            Action act = () => _lookupToRegistrationMapper.Map(registration, null);


            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("secondSource");
        }
        
        [TestMethod]
        public void
            MapRegistrationLookupResponseToOrganDonationRegistration_WhenPassingNullResponseEntry_ThrowsArgumentNullException()
        {
            // Arrange
            var registration = _fixture.Create<OrganDonationRegistration>();
            var response = new RegistrationLookupResponse
            {
                Entry = null
            };

            // Act and Assert
            Action act = () => _lookupToRegistrationMapper.Map(registration, null);


            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("secondSource");
        }
        
        [TestMethod]
        public void
            MapRegistrationLookupResponseToOrganDonationRegistration_WhenPassingNullResponseResource_ThrowsArgumentNullException()
        {
            // Arrange
            var registration = _fixture.Create<OrganDonationRegistration>();
            var response = new RegistrationLookupResponse
            {
                Entry = new List<Entry<Registration>>()
            };

            // Act and Assert
            Action act = () => _lookupToRegistrationMapper.Map(registration, null);


            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("secondSource");
        }

        [TestMethod]
        public void MapRegistrationLookupResponseToOrganDonationRegistration_WithAllValues_MapsCorrectly()
        {
            // Arrange
            var response = _fixture.Create<RegistrationLookupResponse>();
            var resource = response.Entry.First().Resource;
            resource.DonationWishes = new Dictionary<string, string>
            {
                { "all", "yes" },
                { "pancreas", "no" },
                { "heart", "not-stated" },
                { "tissue", "yes" }
            };

            var registration = _fixture.Create<OrganDonationRegistration>();

            _decisionMapper.Setup(x => x.To(resource.OrganDonationDecision)).Returns(Decision.OptIn);
            _faithDeclarationMapper.Setup(x => x.To(resource.FaithDeclaration)).Returns(FaithDeclaration.No);
            _choiceStateMapper.Setup(x => x.To(resource.DonationWishes.First().Value)).Returns(ChoiceState.Yes);
            _choiceStateMapper.Setup(x => x.To(resource.DonationWishes.ElementAt(1).Value)).Returns(ChoiceState.No);
            _choiceStateMapper.Setup(x => x.To(resource.DonationWishes.ElementAt(2).Value))
                .Returns(ChoiceState.NotStated);
            _choiceStateMapper.Setup(x => x.To(resource.DonationWishes.ElementAt(3).Value)).Returns(ChoiceState.Yes);

            // Act
            var result = _lookupToRegistrationMapper.Map(registration, response);

            // Assert
            result.Should().NotBeNull();
            result.NameFull.Should().Be(registration.NameFull);
            result.Name.Should().NotBeNull();
            result.Name.GivenName.Should().Be(registration.Name.GivenName);
            result.Name.Surname.Should().Be(registration.Name.Surname);
            result.Name.Title.Should().Be(registration.Name.Title);
            result.AddressFull.Should().Be(registration.AddressFull);
            result.Address.Should().BeEquivalentTo(registration.Address);
            result.DateOfBirth.Should().Be(registration.DateOfBirth);
            result.NhsNumber.Should().Be(registration.NhsNumber);
            result.Gender.Should().Be(registration.Gender);

            _decisionMapper.Verify(x => x.To(resource.OrganDonationDecision));
            _decisionMapper.VerifyNoOtherCalls();
            _faithDeclarationMapper.Verify(x => x.To(resource.FaithDeclaration));
            _faithDeclarationMapper.VerifyNoOtherCalls();
            _choiceStateMapper.Verify(x => x.To(resource.DonationWishes.First().Value));
            _choiceStateMapper.Verify(x => x.To(resource.DonationWishes.ElementAt(1).Value));
            _choiceStateMapper.Verify(x => x.To(resource.DonationWishes.ElementAt(2).Value));
            _choiceStateMapper.Verify(x => x.To(resource.DonationWishes.ElementAt(3).Value));
            _choiceStateMapper.VerifyNoOtherCalls();

            result.Identifier.Should().Be(resource.Id);
            result.Decision.Should().Be(Decision.OptIn);
            result.DecisionDetails.Should().NotBeNull();
            result.DecisionDetails.All.Should().Be(true);
            result.DecisionDetails.Choices.Should().HaveCount(3);
            result.DecisionDetails.Choices.Should().Contain("pancreas", ChoiceState.No);
            result.DecisionDetails.Choices.Should().Contain("heart", ChoiceState.NotStated);
            result.DecisionDetails.Choices.Should().Contain("tissue", ChoiceState.Yes);
            result.FaithDeclaration.Should().Be(FaithDeclaration.No);
            result.State.Should().Be(State.Ok);
        }
        
        [TestMethod]
        public void MapRegistrationLookupResponseToOrganDonationRegistration_WithAppRepDecision_ShouldNotMapDetails()
        {
            // Arrange
            var response = _fixture.Create<RegistrationLookupResponse>();
            var resource = response.Entry.First().Resource;
            resource.OrganDonationDecision = "app-rep";
            resource.DonationWishes = new Dictionary<string, string>
            {
                { "all", "yes" },
                { "pancreas", "no" },
                { "heart", "not-stated" },
                { "tissue", "yes" }
            };
            
            var registration = _fixture.Create<OrganDonationRegistration>();
            _choiceStateMapper.Setup(x => x.To(It.IsAny<string>())).Returns(ChoiceState.No);

            // Act
            var result = _lookupToRegistrationMapper.Map(registration, response);

            // Assert
            result.Should().NotBeNull();
            result.DecisionDetails.Should().BeNull();
            _choiceStateMapper.VerifyNoOtherCalls();
        }
        
        [TestMethod]
        public void MapRegistrationLookupResponseToOrganDonationRegistration_WithOptOutDecision_ShouldNotMapDetails()
        {
            // Arrange
            var response = _fixture.Create<RegistrationLookupResponse>();
            var resource = response.Entry.First().Resource;
            resource.DonationWishes = new Dictionary<string, string>
            {
                { "all", "yes" },
                { "pancreas", "no" },
                { "heart", "not-stated" },
                { "tissue", "yes" }
            };
            
            var registration = _fixture.Create<OrganDonationRegistration>();
            _decisionMapper.Setup(x => x.To(resource.OrganDonationDecision)).Returns(Decision.OptOut);
            _choiceStateMapper.Setup(x => x.To(It.IsAny<string>())).Returns(ChoiceState.No);

            // Act
            var result = _lookupToRegistrationMapper.Map(registration, response);

            // Assert
            result.Should().NotBeNull();
            result.DecisionDetails.Should().BeNull();
            _choiceStateMapper.VerifyNoOtherCalls();
        }
    }
}
