using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.OrganDonation;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Mappers;
using NHSOnline.Backend.Support;
using Address = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Address;
using Name = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Name;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation
{
    [TestClass]
    public class RegistrationRequestMapperTests
    {
        private IFixture _fixture;
        private IMapper<OrganDonationRegistrationRequest, RegistrationRequest> _registrationRequestMapper;

        private Mock<IEnumMapper<string, Decision>> _decisionMapperMock;
        private Mock<IEnumMapper<string, FaithDeclaration>> _faithDeclarationMapperMock;
        private Mock<IMapper<PfsApi.OrganDonation.Models.Name, Name>> _nameMapperMock;
        private Mock<IMapper<string, PfsApi.OrganDonation.Models.Address, Address>> _addressMapperMock;
        private Mock<IOrganDonationDonationWishesMapper> _donationWishesMapperMock;
        private Mock<IOrganDonationGenderMapper> _genderMapperMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _decisionMapperMock = _fixture.Freeze<Mock<IEnumMapper<string, Decision>>>();
            _faithDeclarationMapperMock = _fixture.Freeze<Mock<IEnumMapper<string, FaithDeclaration>>>();
            _nameMapperMock = _fixture.Freeze<Mock<IMapper<PfsApi.OrganDonation.Models.Name, Name>>>();
            _addressMapperMock =
                _fixture.Freeze<Mock<IMapper<string, PfsApi.OrganDonation.Models.Address, Address>>>();
            _donationWishesMapperMock = _fixture.Freeze<Mock<IOrganDonationDonationWishesMapper>>();
            _genderMapperMock = _fixture.Freeze<Mock<IOrganDonationGenderMapper>>();

            _registrationRequestMapper = _fixture.Create<RegistrationRequestMapper>();
        }

        [TestMethod]
        public void MapToRegistrationRequest_WhenPassingNull_ThrowsAggregateException()
        {
            // Act and Assert
            Action act = () => _registrationRequestMapper.Map(null);

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(3)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x => ((ArgumentNullException) x).ParamName.Equals("source", StringComparison.Ordinal))
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("AdditionalDetails", StringComparison.Ordinal))
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("Registration", StringComparison.Ordinal));
        }

        [TestMethod]
        public void MapToRegistrationRequest_WhenNotPassingRegistrationAndAddtionalDetails_ThrowsAggregateException()
        {
            // Act and Assert
            Action act = () => _registrationRequestMapper.Map(new OrganDonationRegistrationRequest());

            act.Should().Throw<AggregateException>()
                .And.InnerExceptions.Should().HaveCount(2)
                .And.AllBeOfType<ArgumentNullException>()
                .And.Contain(x =>
                    ((ArgumentNullException) x).ParamName.Equals("AdditionalDetails", StringComparison.Ordinal))
                .And.Contain(
                    x => ((ArgumentNullException) x).ParamName.Equals("Registration", StringComparison.Ordinal));
        }

        [TestMethod]
        public void MapToRegistrationRequest_WhenNotPassingAdditionalDetails_ThrowsArgumentNullException()
        {
            // Act and Assert
            Action act = () => _registrationRequestMapper.Map(new OrganDonationRegistrationRequest
            {
                Registration = new OrganDonationStoreRegistration()
            });

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("AdditionalDetails");
        }

        [TestMethod]
        public void MapToRegistrationRequest_WhenNotPassingRegistration_ThrowsArgumentNullException()
        {
            // Act and Assert
            Action act = () => _registrationRequestMapper.Map(new OrganDonationRegistrationRequest
            {
                AdditionalDetails = new AdditionalDetails()
            });

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("Registration");
        }

        [TestMethod]
        public void
            MapToRegistrationRequest_WhenPassingOptInRegistrationWithoutFaithDeclaration_ThrowsArgumentNullException()
        {
            // Arrange 
            var registration = _fixture.Create<OrganDonationStoreRegistration>();
            registration.Decision = Decision.OptIn;
            registration.FaithDeclaration = null;

            // Act and Assert
            Action act = () => _registrationRequestMapper.Map(new OrganDonationRegistrationRequest
            {
                AdditionalDetails = new AdditionalDetails(),
                Registration = registration
            });

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("FaithDeclaration");
        }

        [TestMethod]
        public void MapToRegistrationRequest_WithOptIn_MapsCorrectly()
        {
            // Arrange
            var response = new OrganDonationRegistrationRequest
            {
                AdditionalDetails = _fixture.Create<AdditionalDetails>(),
                Registration = new OrganDonationStoreRegistration()
                {
                    Decision = Decision.OptIn,
                    FaithDeclaration = FaithDeclaration.Yes,
                    Gender = "male",
                    DateOfBirth = new DateTime(1955, 11, 05),
                    NhsNumber = _fixture.Create<string>(),
                }
            };

            var donationWishes = new Dictionary<string, string>
            {
                { "all", "yes" },
                { "heart", "no" }
            };

            var name = new Name
            {
                Given = new List<string> { "John" },
                Family = "Doe",
                Prefix = new List<string> { "Mr" }
            };

            var address = new Address
            {
                Line = new List<string> { "address line" },
                PostalCode = "Postcode"
            };

            _decisionMapperMock.Setup(x => x.From(It.IsAny<Decision>())).Returns("opt-in");
            _donationWishesMapperMock.Setup(x => x.Map(It.IsAny<DecisionDetails>())).Returns(donationWishes);
            _faithDeclarationMapperMock.Setup(x => x.From(It.IsAny<FaithDeclaration>())).Returns("yes");
            _nameMapperMock.Setup(x => x.Map(It.IsAny<PfsApi.OrganDonation.Models.Name>()))
                .Returns(name);
            _addressMapperMock
                .Setup(x => x.Map(It.IsAny<string>(), It.IsAny<PfsApi.OrganDonation.Models.Address>()))
                .Returns(address);
            _genderMapperMock.Setup(x => x.Map(It.IsAny<string>())).Returns("male");

            // Act
            var result = _registrationRequestMapper.Map(response);

            // Assert
            _decisionMapperMock.Verify(x => x.From(It.IsAny<Decision>()));
            _donationWishesMapperMock.Verify(x => x.Map(It.IsAny<DecisionDetails>()));
            _faithDeclarationMapperMock.Verify(x => x.From(It.IsAny<FaithDeclaration>()));
            _nameMapperMock.Verify(x => x.Map(It.IsAny<PfsApi.OrganDonation.Models.Name>()));
            _addressMapperMock.Verify(x =>
                x.Map(It.IsAny<string>(), It.IsAny<PfsApi.OrganDonation.Models.Address>()));
            _genderMapperMock.Verify(x => x.Map(It.IsAny<string>()));

            result.Should().NotBeNull();
            result.OrganDonationDecision.Should().Be("opt-in");
            result.DonationWishes.Should().BeEquivalentTo(donationWishes);
            result.Name.Should().ContainSingle().Which.Should().Be(name);
            result.FaithDeclaration.Should().Be("yes");
            result.Gender.Should().Be("male");
            result.Address.Should().ContainSingle().Which.Should().Be(address);
            result.EthnicCategory.Should().NotBeNull();
            result.EthnicCategory.Coding.Should().ContainSingle().Which.Code.Should()
                .Be(response.AdditionalDetails.EthnicityId);
            result.BirthDate.Should().Be("1955-11-05");
            result.ReligiousAffiliation.Should().NotBeNull();
            result.ReligiousAffiliation.Coding.Should().ContainSingle().Which.Code.Should()
                .Be(response.AdditionalDetails.ReligionId);
            result.Identifier.Should().ContainSingle().Which.Value.Should().Be(response.Registration.NhsNumber);
        }
        
        
        [TestMethod]
        public void MapToRegistrationRequest_MapsAdditionalDetails_Correctly()
        {
            // Arrange
            var response = new OrganDonationRegistrationRequest
            {
                AdditionalDetails = new AdditionalDetails
                {
                    EthnicityId = _fixture.Create<string>(),
                    ReligionId = _fixture.Create<string>()
                },
                
                Registration = _fixture.Create<OrganDonationStoreRegistration>()
            };

            // Act
            var result = _registrationRequestMapper.Map(response);

            // Assert
            result.Should().NotBeNull();
            result.EthnicCategory.Should().NotBeNull();
            result.EthnicCategory.Coding.Should().ContainSingle().Which.Code.Should()
                .Be(response.AdditionalDetails.EthnicityId);
            result.ReligiousAffiliation.Should().NotBeNull();
            result.ReligiousAffiliation.Coding.Should().ContainSingle().Which.Code.Should()
                .Be(response.AdditionalDetails.ReligionId);
        }
        
        [TestMethod]
        [DataRow("", "")]
        [DataRow(null, null)]
        public void MapToRegistrationRequest_MapsAdditionalDetailsWhenEmpty_ToNull(string ethnicityId, string religionId)
        {
            // Arrange
            var response = new OrganDonationRegistrationRequest
            {
                AdditionalDetails = new AdditionalDetails
                {
                    EthnicityId = ethnicityId,
                    ReligionId = religionId
                },
                
                Registration = _fixture.Create<OrganDonationStoreRegistration>()
            };

            // Act
            var result = _registrationRequestMapper.Map(response);

            // Assert
            result.Should().NotBeNull();
            result.EthnicCategory.Should().BeNull();
            result.ReligiousAffiliation.Should().BeNull();
        }

        [TestMethod]
        public void MapToRegistrationRequest_WithOptOut_MapsCorrectly()
        {
            SuccessfulOptOutTest();
        }

        [TestMethod]
        public void MapToRegistrationRequest_WithId_MapsCorrectly()
        {
            var result = SuccessfulOptOutTest("OrganDonationIdentifier");
            result.Id.Should().Be("OrganDonationIdentifier");
        }

        [TestMethod]
        public void MapToRegistrationRequest_WithoutId_MapsCorrectly()
        {
            var result = SuccessfulOptOutTest(null);
            result.Id.Should().BeNullOrEmpty();
        }

        private RegistrationRequest SuccessfulOptOutTest(string organDonationId = null)
        {
            // Arrange
            var response = new OrganDonationRegistrationRequest
            {
                AdditionalDetails = _fixture.Create<AdditionalDetails>(),
                Registration = new OrganDonationStoreRegistration()
                {
                    Identifier = organDonationId,
                    Decision = Decision.OptOut,
                    FaithDeclaration = FaithDeclaration.Yes,
                    Gender = "male",
                    DateOfBirth = new DateTime(1955, 11, 05),
                    NhsNumber = _fixture.Create<string>(),
                }
            };

            var donationWishes = new Dictionary<string, string>
            {
                { "all", "yes" },
                { "heart", "no" }
            };

            var name = new Name
            {
                Given = new List<string> { "John" },
                Family = "Doe",
                Prefix = new List<string> { "Mr" }
            };

            var address = new Address
            {
                Line = new List<string> { "address line" },
                PostalCode = "Postcode"
            };

            _decisionMapperMock.Setup(x => x.From(It.IsAny<Decision>())).Returns("opt-out");
            _donationWishesMapperMock.Setup(x => x.Map(It.IsAny<DecisionDetails>())).Returns(donationWishes);
            _faithDeclarationMapperMock.Setup(x => x.From(It.IsAny<FaithDeclaration>())).Returns("yes");
            _nameMapperMock.Setup(x => x.Map(It.IsAny<PfsApi.OrganDonation.Models.Name>()))
                .Returns(name);
            _addressMapperMock
                .Setup(x => x.Map(It.IsAny<string>(), It.IsAny<PfsApi.OrganDonation.Models.Address>()))
                .Returns(address);
            _genderMapperMock.Setup(x => x.Map(It.IsAny<string>())).Returns("male");

            // Act
            var result = _registrationRequestMapper.Map(response);

            // Assert
            _decisionMapperMock.Verify(x => x.From(It.IsAny<Decision>()));
            _donationWishesMapperMock.VerifyNoOtherCalls();
            _faithDeclarationMapperMock.VerifyNoOtherCalls();
            _nameMapperMock.Verify(x => x.Map(It.IsAny<PfsApi.OrganDonation.Models.Name>()));
            _addressMapperMock.Verify(x =>
                x.Map(It.IsAny<string>(), It.IsAny<PfsApi.OrganDonation.Models.Address>()));
            _genderMapperMock.Verify(x => x.Map(It.IsAny<string>()));

            result.Should().NotBeNull();
            result.OrganDonationDecision.Should().Be("opt-out");
            result.DonationWishes.Should().BeNull();
            result.Name.Should().ContainSingle().Which.Should().Be(name);
            result.FaithDeclaration.Should().BeNull();
            result.Gender.Should().Be("male");
            result.Address.Should().ContainSingle().Which.Should().Be(address);
            result.EthnicCategory.Should().NotBeNull();
            result.EthnicCategory.Coding.Should().ContainSingle().Which.Code.Should()
                .Be(response.AdditionalDetails.EthnicityId);
            result.BirthDate.Should().Be("1955-11-05");
            result.ReligiousAffiliation.Should().NotBeNull();
            result.ReligiousAffiliation.Coding.Should().ContainSingle().Which.Code.Should()
                .Be(response.AdditionalDetails.ReligionId);
            result.Identifier.Should().ContainSingle().Which.Value.Should().Be(response.Registration.NhsNumber);

            return result;
        }
    }
}