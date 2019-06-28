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
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Linkage
{
    [TestClass]
    public class VisionLinkagePostLinkageErrorMapperTests
    {
        private VisionLinkageMapperTestHelpers _helper;
        private Mock<ILogger<VisionLinkageService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _helper = new VisionLinkageMapperTestHelpers(fixture);
            _logger = fixture.Create<Mock<ILogger<VisionLinkageService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => VisionLinkagePostErrorMapper.Map(null, _logger.Object);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithConflictValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<LinkageKeyPostResponse>(HttpStatusCode.Conflict,
                "V2214");

            // Act
            var result = VisionLinkagePostErrorMapper.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>();
            var conflictResult = (LinkageResult.ErrorCase) result;
            conflictResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.LinkageKeyAlreadyExists);
        }
        
        [TestMethod]
        public void Map_WithUnknownValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<LinkageKeyPostResponse>(HttpStatusCode.Conflict, "10");

            // Act
            var result = VisionLinkagePostErrorMapper.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.UnmappedErrorWithStatusCode>();
            var linkageResult = (LinkageResult.UnmappedErrorWithStatusCode)result;
            linkageResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }
    }
}