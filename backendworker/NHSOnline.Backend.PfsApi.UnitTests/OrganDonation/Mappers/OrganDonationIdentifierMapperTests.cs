using System;
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
    public class OrganDonationIdentifierMapperTests
    {
        private IMapper<string, Identifier> _organDonationIdentifierMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _organDonationIdentifierMapper = fixture.Create<OrganDonationIdentifierMapper>();
        }

        [TestMethod]
        public void MapToIdentifier_WhenPassingNoNhsNumber_ThrowsAnException()
        {
            // Act and Assert
            Action act = () => _organDonationIdentifierMapper.Map(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void MapToIdentifier_WhenPassingNhsNumberWithWhiteSpace_TrimsWhiteSpace()
        {
            // Act and Assert
            var result = _organDonationIdentifierMapper.Map(" 123 456 7891 ");
            result.Should().NotBeNull();
            result.System.Should().Be(Constants.OrganDonationConstants.IdentifierSystem);
            result.Value.Should().Be("1234567891");
        }
    }
}