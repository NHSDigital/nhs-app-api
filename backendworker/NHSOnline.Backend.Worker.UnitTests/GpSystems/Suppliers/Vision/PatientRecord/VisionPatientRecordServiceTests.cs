using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord
{
    [TestClass]
    public class VisionPatientRecordServiceTests
    {
        private const string DefaultConnectionToken = "{\"rosuAccountId\":\"account_id\",\"apiKey\":\"key\"}";
        private const string DefaultOdsCode = "token";
        private IFixture _fixture;
        private Mock<IVisionClient> _mockVisionClient;
        private ILogger<VisionPatientRecordService> _logger;
        private IVisionMyRecordMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = Mock.Of<ILogger<VisionPatientRecordService>>();
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _mapper = Mock.Of<IVisionMyRecordMapper>();
        }

        [TestMethod]
        public async Task Get_InvalidRequest_ReturnsInvalidRequestResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionClient.VisionApiObjectResponse<PatientConfiguration>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfiguration>
                        {
                            Body = new VisionResponseBody<PatientConfiguration>
                            {
                                Fault = new Fault
                                {
                                    Detail = new Detail
                                    {
                                        VisionFault = new VisionFault
                                        {
                                            Error = new FaultError
                                            {
                                                Category = "INVALID_REQUEST",
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    }));

            var systemUnderTest = new VisionPatientRecordService(_logger, _mockVisionClient.Object, _mapper);

            var visionUserSession = new VisionUserSession
            {
                RosuAccountId = DefaultConnectionToken,
                OdsCode = DefaultOdsCode,
            };
            
            // Act
            var result = await systemUnderTest.Get(visionUserSession);

            // Assert
            result.Should().BeAssignableTo<GetMyRecordResult.InvalidRequest>();
        }
        
        [TestMethod]
        public async Task Get_InvalidUserCredentials_ReturnsInvalidUserCredentialsResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionClient.VisionApiObjectResponse<PatientConfiguration>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfiguration>
                        {
                            Body = new VisionResponseBody<PatientConfiguration>
                            {
                                VisionResponse = new VisionResponse<PatientConfiguration>
                                {
                                    ServiceHeader = new ServiceHeaderResponse
                                    {
                                        Outcome = new Outcome
                                        {
                                            Successful = bool.FalseString.ToLower(),
                                            Error = new OutcomeError
                                            {
                                                Code = "-30",
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    }));

            var systemUnderTest = new VisionPatientRecordService(_logger, _mockVisionClient.Object, _mapper);

            var visionUserSession = new VisionUserSession
            {
                RosuAccountId = DefaultConnectionToken,
                OdsCode = DefaultOdsCode,
            };

            // Act
            var result = await systemUnderTest.Get(visionUserSession);

            // Assert
            result.Should().BeAssignableTo<GetMyRecordResult.InvalidUserCredentials>();
        }

        [TestMethod]
        public async Task Get_InvalidSecurityHeader_ReturnsErrorProcessingSecurityHeaderResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionClient.VisionApiObjectResponse<PatientConfiguration>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfiguration>
                        {
                            Body = new VisionResponseBody<PatientConfiguration>
                            {
                                Fault = new Fault
                                {
                                    FaultCode = "ns1:InvalidSecurity",
                                },
                            },
                        },
                    }));

            var systemUnderTest = new VisionPatientRecordService(_logger, _mockVisionClient.Object, _mapper);

            var visionUserSession = new VisionUserSession
            {
                RosuAccountId = DefaultConnectionToken,
                OdsCode = DefaultOdsCode,
            };

            // Act
            var result = await systemUnderTest.Get(visionUserSession);

            // Assert
            result.Should().BeAssignableTo<GetMyRecordResult.ErrorProcessingSecurityHeader>();
        }

        [TestMethod]
        public async Task Get_UnknownError_ReturnsUnknownErrorResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionClient.VisionApiObjectResponse<PatientConfiguration>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfiguration>
                        {
                            Body = new VisionResponseBody<PatientConfiguration>
                            {
                                VisionResponse = new VisionResponse<PatientConfiguration>
                                {
                                    ServiceHeader = new ServiceHeaderResponse
                                    {
                                        Outcome = new Outcome
                                        {
                                            Successful = bool.FalseString.ToLower(),
                                            Error = new OutcomeError
                                            {
                                                Code = "-100",
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    }));

            var systemUnderTest = new VisionPatientRecordService(_logger, _mockVisionClient.Object, _mapper);

            var visionUserSession = new VisionUserSession
            {
                RosuAccountId = DefaultConnectionToken,
                OdsCode = DefaultOdsCode,
            };

            // Act
            var result = await systemUnderTest.Get(visionUserSession);

            // Assert
            result.Should().BeAssignableTo<GetMyRecordResult.UnknownError>();
        }

        [TestMethod]
        public async Task Get_WhenNoMatchedError_ReturnsUnsuccessfulResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionClient.VisionApiObjectResponse<PatientConfiguration>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfiguration>
                        {
                            Body = new VisionResponseBody<PatientConfiguration>
                            {
                                Fault = new Fault(), // fault but no code
                            },
                        },
                    }));

            var systemUnderTest = new VisionPatientRecordService(_logger, _mockVisionClient.Object, _mapper);

            var visionUserSession = new VisionUserSession
            {
                RosuAccountId = DefaultConnectionToken,
                OdsCode = DefaultOdsCode,
            };

            // Act
            var result = await systemUnderTest.Get(visionUserSession);

            // Assert
            result.Should().BeAssignableTo<GetMyRecordResult.Unsuccessful>();
        }
    }
}