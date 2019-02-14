using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Linkage
{
    [TestClass]
    public class VisionLinkageMapperTests
    {
        private IFixture _fixture;
        private IVisionLinkageMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new VisionLinkageMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void MapLinkageKeyGetResponseToLinkageResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map((LinkageKeyGetResponse)null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void MapLinkageKeyGetResponseToLinkageResponse_WithValues_MapsCorrectly()
        {
            // Arrange
            var response = _fixture.Create<LinkageKeyGetResponse>();

            // Act
            var result = _mapper.Map(response);

            // Assert
            result.Should().NotBeNull();
            result.AccountId.Should().Be(response.AccountId);
            result.LinkageKey.Should().Be(response.LinkageKey);
            result.OdsCode.Should().Be(response.OdsCode);
        }
        
        [TestMethod]
        public void MapLinkageKeyPostResponseToLinkageResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map((LinkageKeyPostResponse)null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void MapLinkageKeyPostResponseToLinkageResponse_WithValues_MapsCorrectly()
        {
            // Arrange
            var response = _fixture.Create<LinkageKeyPostResponse>();

            // Act
            var result = _mapper.Map(response);

            // Assert
            result.Should().NotBeNull();
            result.AccountId.Should().Be(response.AccountId);
            result.LinkageKey.Should().Be(response.LinkageKey);
            result.OdsCode.Should().Be(response.OdsCode);
        }
    }
}