using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Linkage;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Im1Connection
{
    [TestClass]
    public class EmisIm1RegisterErrorMapperTests
    {
        private EmisIm1RegisterErrorMapper _mapperUnderTest;
        private IFixture _fixture;
        private EmisTestHelpers _helper;
        private Mock<ILogger<EmisIm1ConnectionService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _mapperUnderTest = _fixture.Create<EmisIm1RegisterErrorMapper>();
            _helper = new EmisTestHelpers(_fixture);
            _logger = _fixture.Create<Mock<ILogger<EmisIm1ConnectionService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapperUnderTest.Map(null, _logger.Object);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithForbiddenValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<MeApplicationsPostResponse>(HttpStatusCode.Forbidden, 1030, "Patient Facing Services API v2 is not enabled at this practice");

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>();
            var conflictResult = (Im1ConnectionRegisterResult.ErrorCase) result;
            conflictResult.ErrorCode.Should().Be((int)Im1ConnectionErrorCodes.Code.PatientFacingServicesApiv2IsNotEnabledAtThisPractice);
        }

        [TestMethod]
        public void Map_WithNotFoundValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<MeApplicationsPostResponse>(HttpStatusCode.NotFound, 1104, null);

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>();
            var conflictResult = (Im1ConnectionRegisterResult.ErrorCase) result;
            conflictResult.ErrorCode.Should().Be((int)Im1ConnectionErrorCodes.Code.NoUserFoundForLinkageDetails);
        }

        [TestMethod]
        public void Map_WithBadRequestValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<MeApplicationsPostResponse>(HttpStatusCode.BadRequest, 1105, null);

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>();
            var conflictResult = (Im1ConnectionRegisterResult.ErrorCase)result;
            conflictResult.ErrorCode.Should().Be((int)Im1ConnectionErrorCodes.Code.InvalidLinkageDetails);
        }

        [TestMethod]
        public void Map_WithBadRequestValuesAndMessage_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<MeApplicationsPostResponse>(HttpStatusCode.BadRequest, null, "LinkageKey length outside of valid range.");

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>();
            var conflictResult = (Im1ConnectionRegisterResult.ErrorCase)result;
            conflictResult.ErrorCode.Should().Be((int)Im1ConnectionErrorCodes.Code.LinkageKeyLengthOutsideOfValidRange);
        }

        [TestMethod]
        public void Map_WithBadRequestValuesAndUnknownMessage_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<MeApplicationsPostResponse>(HttpStatusCode.BadRequest, null, "Something that doesn't match");

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.BadRequest>();
            var conflictResult = (Im1ConnectionRegisterResult.BadRequest)result;
            conflictResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.Code.UnknownError);
        }

        [TestMethod]
        public void Map_WithUnknownValues_MapsCorrectly()
        {
            // Arrange
            var response = _helper.CreateResponse<MeApplicationsPostResponse>(HttpStatusCode.BadRequest, 999, null);

            // Act
            var result = _mapperUnderTest.Map(response, _logger.Object);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.BadRequest>();
            var conflictResult = (Im1ConnectionRegisterResult.BadRequest) result;
            conflictResult.ErrorCode.Should().Be((int) Im1ConnectionErrorCodes.Code.UnknownError);
        }
    }
}