using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Mappers;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support;
using Address = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Address;
using Name = NHSOnline.Backend.PfsApi.OrganDonation.ApiModels.Name;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation.Mappers
{
    [TestClass]
    public class WithdrawRequestMapperTests
    {
        private IFixture _fixture;
        private IMapper<OrganDonationWithdrawRequest, WithdrawRequest> _withdrawRequestMapper;
        private Mock<IMapper<PfsApi.OrganDonation.Models.Name, Name>> _nameMapperMock;
        private Mock<IMapper<string, PfsApi.OrganDonation.Models.Address, Address>> _addressMapperMock;
        private Mock<IOrganDonationGenderMapper> _genderMapperMock;
        private Mock<IOrganDonationIdentifierMapper> _identifierMapperMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _nameMapperMock = _fixture.Freeze<Mock<IMapper<PfsApi.OrganDonation.Models.Name, Name>>>();
            _addressMapperMock =
                _fixture.Freeze<Mock<IMapper<string, PfsApi.OrganDonation.Models.Address, Address>>>();
            _genderMapperMock = _fixture.Freeze<Mock<IOrganDonationGenderMapper>>();
            _identifierMapperMock = _fixture.Freeze<Mock<IOrganDonationIdentifierMapper>>();

            _withdrawRequestMapper = _fixture.Create<WithdrawRequestMapper>();
        }


        [TestMethod]
        public void MapToWithdrawRequest_WithValidRequest_MapsCorrectly()
        {
            SuccessfulTest();
        }

        [TestMethod]
        public void MapToWithdrawRequest_WithId_MapsCorrectly()
        {
            var result = SuccessfulTest("OrganDonationIdentifier");
            result.Id.Should().Be("OrganDonationIdentifier");
        }

        [TestMethod]
        public void MapToWithdrawRequest_WithoutId_MapsCorrectly()
        {
            var result = SuccessfulTest();
            result.Id.Should().BeNullOrEmpty();
        }

        private WithdrawRequest SuccessfulTest(string organDonationId = null)
        {
            // Arrange
            var response = new OrganDonationWithdrawRequest
            {
                Identifier = organDonationId,
                Gender = "male",
                DateOfBirth = new DateTime(1955, 11, 05),
                NhsNumber = _fixture.Create<string>(),
                WithdrawReasonId = "WithdrawReason"
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

            var identifier = new Identifier
            {
                System = Constants.OrganDonationConstants.IdentifierSystem,
                Value = response.NhsNumber.RemoveWhiteSpace()
            };

            _nameMapperMock.Setup(x => x.Map(It.IsAny<PfsApi.OrganDonation.Models.Name>()))
                .Returns(name);
            _addressMapperMock
                .Setup(x => x.Map(It.IsAny<string>(), It.IsAny<PfsApi.OrganDonation.Models.Address>()))
                .Returns(address);
            _genderMapperMock.Setup(x => x.Map(It.IsAny<string>())).Returns("male");
            _identifierMapperMock.Setup(x => x.Map(It.IsAny<string>())).Returns(identifier);

            // Act
            var result = _withdrawRequestMapper.Map(response);

            // Assert
            _nameMapperMock.Verify(x => x.Map(It.IsAny<PfsApi.OrganDonation.Models.Name>()));
            _addressMapperMock.Verify(x =>
                x.Map(It.IsAny<string>(), It.IsAny<PfsApi.OrganDonation.Models.Address>()));
            _genderMapperMock.Verify(x => x.Map(It.IsAny<string>()));
            _identifierMapperMock.Verify(x => x.Map(It.IsAny<string>()));

            result.Should().NotBeNull();
            result.Name.Should().ContainSingle().Which.Should().Be(name);
            result.Gender.Should().Be("male");
            result.Address.Should().ContainSingle().Which.Should().Be(address);
            result.BirthDate.Should().Be("1955-11-05");
            result.Identifier.Should().ContainSingle().Which.Value.Should().Be(response.NhsNumber);
            result.WithdrawReason.Coding.First().Code.Should().Be("WithdrawReason");
            result.WithdrawReason.Coding.First().System.Should()
                .Be(Constants.OrganDonationConstants.WithdrawReasonCodingSystem);

            return result;
        }
    }
}