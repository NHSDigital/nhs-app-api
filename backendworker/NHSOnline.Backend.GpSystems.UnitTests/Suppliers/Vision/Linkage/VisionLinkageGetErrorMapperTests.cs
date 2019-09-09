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
    public class VisionLinkageGetErrorMapperTests
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
            Action act = () => VisionLinkageGetErrorMapper.Map(null, _logger.Object);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithNotFoundValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<LinkageKeyGetResponse>(HttpStatusCode.NotFound,
                "VY806");

            // Act
            var result = VisionLinkageGetErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.PatientRecordNotFound);
        }

        [TestMethod]
        public void Map_WithNotFoundValuesWithMessage_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<LinkageKeyGetResponse>(HttpStatusCode.NotFound,
                "V2210",
                "No user associated with the nhs number");

            // Act
            var result = VisionLinkageGetErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.NoUserAssociatedWithNhsNumber);
        }

        [TestMethod]
        public void Map_WithNotFoundValuesWithNotMatchingMessage_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<LinkageKeyGetResponse>(HttpStatusCode.NotFound,
                "V2210",
                "Something else");

            // Act
            var result = VisionLinkageGetErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<LinkageResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }

        [TestMethod]
        public void Map_WithBadRequestValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<LinkageKeyGetResponse>(HttpStatusCode.BadRequest,
                "V4205");

            // Act
            var result = VisionLinkageGetErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be( Im1ConnectionErrorCodes.InternalCode.InvalidNhsNumber);
        }

        [TestMethod]
        public void Map_WithUnknownValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<LinkageKeyGetResponse>(HttpStatusCode.BadRequest, "999");

            // Act
            var result = VisionLinkageGetErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<LinkageResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }
    }
}