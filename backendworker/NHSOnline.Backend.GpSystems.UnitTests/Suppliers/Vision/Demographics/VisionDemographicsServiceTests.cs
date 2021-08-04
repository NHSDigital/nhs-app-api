using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Demographics
{
    [TestClass]
    public class VisionDemographicsServiceTests
    {

        private IFixture _fixture;
        private Mock<IVisionClient> _mockVisionClient;
        private Mock<IVisionDemographicsMapper> _mockVisionMapper;
        private ILoggerFactory _mockLogger;
        private VisionUserSession _visionUserSession;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();

            _mockVisionMapper = _fixture.Freeze<Mock<IVisionDemographicsMapper>>();

            _mockLogger = _fixture.Create<ILoggerFactory>();

            _visionUserSession = _fixture.Create<VisionUserSession>();
        }

        [TestMethod]
        public async Task GetDemographics_WhenValidRequest_ReturnsSuccessResult()
        {
            _mockVisionClient.Setup(x =>
                    x.GetDemographicsV2(It.IsAny<VisionUserSession>(), It.IsAny<DemographicsRequest>()))
                .Returns(Task.FromResult(
                    new VisionDirectServicesApiObjectResponse<VisionDemographicsResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponse<VisionDemographicsResponse>
                        {
                            ServiceContent = new VisionDemographicsResponse
                            {
                                Demographics = new VisionDemographics()
                                {
                                }
                            }
                        }
                    }));

            var demographicsResponse = new DemographicsResponse();

            _mockVisionMapper.Setup(x =>
                x.Map(It.IsAny<VisionDemographics>(), _visionUserSession.NhsNumber))
                .Returns(demographicsResponse);

            var expectedResult = new DemographicsResult.Success(demographicsResponse);

            var systemUnderTest = CreateVisionDemographicsService();

            var gpLinkedAccountModel = new GpLinkedAccountModel(_visionUserSession);

            // Act
            var result = await systemUnderTest.GetDemographics(gpLinkedAccountModel);

            // Assert
            _mockVisionClient.VerifyAll();

            result.Should().BeAssignableTo<DemographicsResult.Success>().Subject
                .Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task GetDemographics_WhenInternalServerError_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetDemographicsV2(It.IsAny<VisionUserSession>(), It.IsAny<DemographicsRequest>()))
                .Returns(Task.FromResult(
                    new VisionDirectServicesApiObjectResponse<VisionDemographicsResponse>(HttpStatusCode.InternalServerError)
                    {
                    }));

            var systemUnderTest = CreateVisionDemographicsService();

            var gpLinkedAccountModel = new GpLinkedAccountModel(_visionUserSession);

            // Act
            var result = await systemUnderTest.GetDemographics(gpLinkedAccountModel);

            // Assert
            _mockVisionClient.VerifyAll();

            result.Should().BeAssignableTo<DemographicsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetDemographics_WhenAccessDenied_ReturnsForbiddenResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetDemographicsV2(It.IsAny<VisionUserSession>(), It.IsAny<DemographicsRequest>()))
                .Returns(Task.FromResult(
                    new VisionDirectServicesApiObjectResponse<VisionDemographicsResponse>(HttpStatusCode.BadRequest)
                    {
                        ErrorDetail = new Detail
                        {
                            VisionFault = new VisionFault
                            {
                                Error = new FaultError
                                {
                                    Text = VisionApiErrorCodes.AccessDenied
                                },
                            },
                        },
                    }));

            var systemUnderTest = CreateVisionDemographicsService();

            var gpLinkedAccountModel = new GpLinkedAccountModel(_visionUserSession);

            // Act
            var result = await systemUnderTest.GetDemographics(gpLinkedAccountModel);

            // Assert
            _mockVisionClient.VerifyAll();

            result.Should().BeAssignableTo<DemographicsResult.Forbidden>();
        }

        [TestMethod]
        public async Task GetDemographics_WhenThrowsHttpException_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetDemographicsV2(It.IsAny<VisionUserSession>(), It.IsAny<DemographicsRequest>()))
                .Throws<HttpRequestException>();

            var systemUnderTest = CreateVisionDemographicsService();

            var gpLinkedAccountModel = new GpLinkedAccountModel(_visionUserSession);

            // Act
            var result = await systemUnderTest.GetDemographics(gpLinkedAccountModel);

            // Assert
            _mockVisionClient.VerifyAll();

            result.Should().BeAssignableTo<DemographicsResult.BadGateway>();
        }

        [TestMethod]
        public async Task GetDemographics_WhenMapperThrowsException_ReturnsInternalServerResult()
        {
            _mockVisionClient.Setup(x =>
                    x.GetDemographicsV2(It.IsAny<VisionUserSession>(), It.IsAny<DemographicsRequest>()))
                .Returns(Task.FromResult(
                    new VisionDirectServicesApiObjectResponse<VisionDemographicsResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponse<VisionDemographicsResponse>
                        {
                            ServiceContent = new VisionDemographicsResponse
                            {
                                Demographics = new VisionDemographics()
                            }
                        }
                    }));

            _mockVisionMapper.Setup(x =>
                    x.Map(It.IsAny<VisionDemographics>(), _visionUserSession.NhsNumber))
                .Throws<Exception>();

            var systemUnderTest = CreateVisionDemographicsService();

            var gpLinkedAccountModel = new GpLinkedAccountModel(_visionUserSession);

            // Act
            var result = await systemUnderTest.GetDemographics(gpLinkedAccountModel);

            // Assert
            _mockVisionClient.VerifyAll();

            result.Should().BeAssignableTo<DemographicsResult.InternalServerError>();
        }



        private VisionDemographicsService CreateVisionDemographicsService()
        {
            return new VisionDemographicsService(
                _mockLogger,
                _mockVisionClient.Object,
                _mockVisionMapper.Object);
        }
    }
}
