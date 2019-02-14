using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Linkage
{
    [TestClass]
    public class EmisLinkageMapperTests
    {
        private IFixture _fixture;
        private IEmisLinkageMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new EmisLinkageMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void MapLinkageDetailsResponseToLinkageResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("addVerificationResponse");
        }

        [TestMethod]
        public void MapAddVerificationResponseToLinkageResponse_WithValues_MapsCorrectly()
        {
            // Arrange
            var response = _fixture.Create<AddVerificationResponse>();

            // Act
            var result = _mapper.Map(response);

            // Assert
            result.Should().NotBeNull();
            result.AccountId.Should().Be(response.AccountId);
            result.LinkageKey.Should().Be(response.LinkageKey);
        }
    }
}
