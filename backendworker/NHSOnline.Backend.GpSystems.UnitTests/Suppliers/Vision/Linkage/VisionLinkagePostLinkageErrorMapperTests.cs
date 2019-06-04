using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Linkage
{
    [TestClass]
    public class VisionLinkagePostLinkageErrorMapperTests
    {
        private VisionLinkageMapperTestHelpers _helper;
        private VisionLinkagePostErrorMapper _mapperUnderTest;
        private Mock<ILogger<VisionLinkageService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _helper = new VisionLinkageMapperTestHelpers(fixture);
            _mapperUnderTest = new VisionLinkagePostErrorMapper();
            _logger = fixture.Create<Mock<ILogger<VisionLinkageService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapperUnderTest.Map(null, _logger.Object);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithConflictValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<LinkageKeyPostResponse>(HttpStatusCode.Conflict,
                "V2214");

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>();
            var conflictResult = (LinkageResult.ErrorCase) result;
            conflictResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.LinkageKeyAlreadyExists);
        }
        
        [TestMethod]
        public void Map_WithUnknownValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<LinkageKeyPostResponse>(HttpStatusCode.Conflict, "10");

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.Conflict>();
            var conflictResult = (LinkageResult.Conflict) result;
            conflictResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.UnknownError);
        }
    }
}