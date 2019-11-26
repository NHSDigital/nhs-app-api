using System;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Im1Connection
{
    [TestClass]
    public class VisionIm1RegisterErrorMapperTests
    {
        private IFixture _fixture;
        private Mock<ILogger<VisionIm1ConnectionService>> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _logger = _fixture.Create<Mock<ILogger<VisionIm1ConnectionService>>>();
        }

        [TestMethod]
        public void Map_WhenPassingNull_ThrowsNullReferenceException()
        {
            // Act
            Action act = () => VisionIm1RegisterErrorMapper.Map(null, _logger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("response");
        }

        [TestMethod]
        public void Map_WithOkayWithErrorValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK,
                "-19");

            // Act
            var result = VisionIm1RegisterErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UserAccountDisabled);
        }

        [TestMethod]
        public void Map_WithOkayWithErrorValuesWithUnknownMessage_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK,
                "-100",
                "something else");

            // Act
            var result = VisionIm1RegisterErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.ConnectionToServiceFailed);
        }

        [TestMethod]
        public void Map_WithBadRequestValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.BadRequest,
                "V4205");

            // Act
            var result = VisionIm1RegisterErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.InvalidNhsNumber);
        }

        [TestMethod]
        public void Map_WithNotFoundValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.NotFound,
                "V4205");

            // Act
            var result = VisionIm1RegisterErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.InvalidNhsNumber);
        }

        [TestMethod]
        public void Map_WithOkayWithUnknownErrorValues_MapsCorrectly()
        {
            // Arrange
            var response = CreateResponse(HttpStatusCode.OK, "10");

            // Act
            var result = VisionIm1RegisterErrorMapper.Map(response, _logger.Object);
            
            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }

        private static VisionPFSClient.VisionApiObjectResponse<ServiceContentRegisterResponse> CreateResponse(
            HttpStatusCode statusCode,
            string errorCode,
            string errormessage = null)
        {
            return new VisionPFSClient.VisionApiObjectResponse<ServiceContentRegisterResponse>(statusCode)
            {
                RawResponse = new VisionResponseEnvelope<ServiceContentRegisterResponse>
                {
                    Body = new VisionResponseBody<ServiceContentRegisterResponse>
                    {
                        VisionResponse = new VisionResponse<ServiceContentRegisterResponse>
                        {
                            ServiceHeader = new ServiceHeaderResponse()
                            {
                                Outcome = new Outcome
                                {
                                    Successful = "false",
                                    Error = new OutcomeError()
                                    {
                                        Code = errorCode,
                                        Description = errormessage
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}