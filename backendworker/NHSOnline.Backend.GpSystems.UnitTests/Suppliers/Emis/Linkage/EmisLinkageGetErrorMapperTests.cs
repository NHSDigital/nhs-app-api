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
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Verifications;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Linkage
{
    [TestClass]
    public class EmisLinkageGetErrorMapperTests
    {
        private EmisTestHelpers _helper;
        private Mock<ILogger<EmisLinkageService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _helper = new EmisTestHelpers(fixture);
            _logger = fixture.Freeze<Mock<ILogger<EmisLinkageService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => EmisLinkageGetErrorMapper.Map(null, _logger.Object);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithForbiddenValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<AddVerificationResponse>(HttpStatusCode.Forbidden,
                1030,
                "Patient Facing Services API v2 is not enabled at this practice");

            // Act
            var result = EmisLinkageGetErrorMapper.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>();
            var conflictResult = (LinkageResult.ErrorCase) result;
            conflictResult.ErrorCode.Should().Be( Im1ConnectionErrorCodes.InternalCode.PatientFacingServicesApiv2IsNotEnabledAtThisPractice);
        }

        [TestMethod]
        public void Map_WithNotFoundValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<AddVerificationResponse>(HttpStatusCode.NotFound,
                1551);

            // Act
            var result = EmisLinkageGetErrorMapper.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>();
            var conflictResult = (LinkageResult.ErrorCase) result;
            conflictResult.ErrorCode.Should().Be( Im1ConnectionErrorCodes.InternalCode.PatientNotRegisteredAtThisPractice);
        }

        [TestMethod]
        public void Map_WithBadRequestValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<AddVerificationResponse>(HttpStatusCode.BadRequest,
                1553);

            // Act
            var result = EmisLinkageGetErrorMapper.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.ErrorCase>();
            var conflictResult = (LinkageResult.ErrorCase) result;
            conflictResult.ErrorCode.Should().Be( Im1ConnectionErrorCodes.InternalCode.UnderMinimumAgeOrNonCompetent);
        }

        [TestMethod]
        public void Map_WithUnknownValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<AddVerificationResponse>(HttpStatusCode.BadRequest, 999);

            // Act
            var result = EmisLinkageGetErrorMapper.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<LinkageResult.UnmappedErrorWithStatusCode>();
            var linkageResult = (LinkageResult.UnmappedErrorWithStatusCode)result;
            linkageResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }
    }
}