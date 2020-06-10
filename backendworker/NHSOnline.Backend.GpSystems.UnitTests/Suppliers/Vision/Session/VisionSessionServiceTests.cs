using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Session
{
    [TestClass]
    public class VisionSessionServiceTests
    {
        private const string DefaultConnectionToken = "{\"rosuAccountId\": \"104969\", \"" +
                                                      "apiKey\":\"h4h9869kj3ytz6427y7" +
                                                      "rctkdy3zkpxcncnhvfph76g2h6p9" +
                                                      "gywjq484c9ghan8tt\"}";
        private const string DefaultOdsCode = "token";
        private const string DefaultApiKey = "h4h9869kj3ytz6427y7rctkdy3zkpxcncnhvfph76g2h6p9gywjq484c9ghan8tt";
        private const string DefaultRosuAccountId = "104969";

        private string _nhsNumber;
        private IFixture _fixture;
        private Mock<IVisionClient> _mockVisionClient;
        private ILogger<VisionSessionService> _mockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();

            _nhsNumber = _fixture.Create<string>();

            _mockLogger = _fixture.Create<ILogger<VisionSessionService>>();
        }

        [TestMethod]
        public async Task Create_ValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var accountName = _fixture.Create<string>();
            var patientNumber = _fixture.Create<PatientNumber>();
            var patientId = _fixture.Create<string>();
            patientNumber.NumberType = "NHS";
            var locations = _fixture.CreateMany<Location>().ToList();

            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                        {
                            Body = new VisionResponseBody<PatientConfigurationResponse>
                            {
                                VisionResponse = new VisionResponse<PatientConfigurationResponse>
                                {
                                    ServiceContent = new PatientConfigurationResponse
                                    {
                                        Configuration = new PatientConfiguration
                                        {
                                            Account = new Account
                                            {
                                                Name = accountName,
                                                PatientNumbers = new List<PatientNumber> {patientNumber},
                                                PatientId = patientId 
                                            },
                                            Prescriptions = new PrescriptionsConfiguration
                                            {
                                                RepeatEnabled = true
                                            },
                                            Appointments = new AppointmentsConfiguration
                                            {
                                                BookingEnabled = false
                                            },
                                            References = new PatientReferences
                                            {
                                                Locations = locations
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }));

            var expectedResult = new GpSessionCreateResult.Success(new VisionUserSession
            {
                Name = accountName,
                NhsNumber = _nhsNumber,
                PatientId = patientId,
                OdsCode = DefaultOdsCode,
                ApiKey = DefaultApiKey,
                RosuAccountId = DefaultRosuAccountId,
                IsRepeatPrescriptionsEnabled = true,
                IsAppointmentsEnabled = false,
                LocationIds = locations.Select(l => l.Id).ToList()
            });
            
            var systemUnderTest = CreateVisionSessionService();

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode, _nhsNumber);

            // Assert
            _mockVisionClient.VerifyAll();
            
            result.Should().BeAssignableTo<GpSessionCreateResult.Success>().Subject
                .Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public async Task Create_InvalidRequest_ReturnsBadRequestResult()
        {
            // Arrange            
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                        {
                            Body = new VisionResponseBody<PatientConfigurationResponse>
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

            var systemUnderTest = CreateVisionSessionService();

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode, _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task Create_InvalidUserCredentials_ReturnsForbiddenResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                        {
                            Body = new VisionResponseBody<PatientConfigurationResponse>
                            {
                                VisionResponse = new VisionResponse<PatientConfigurationResponse>
                                {
                                    ServiceHeader = new ServiceHeaderResponse
                                    {
                                        Outcome = new Outcome
                                        {
                                            Successful = "false",
                                            Error = new OutcomeError
                                            {
                                                Code = "-30",
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }));

            var systemUnderTest = CreateVisionSessionService();

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode, _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.Forbidden>();
        }

        [TestMethod]
        public async Task Create_InvalidSecurityHeader_ReturnsInternalServerErrorResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                        {
                            Body = new VisionResponseBody<PatientConfigurationResponse>
                            {
                                Fault = new Fault
                                {
                                    FaultCode = "ns1:InvalidSecurity",
                                },
                            },
                        },
                    }));

            var systemUnderTest = CreateVisionSessionService();

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode, _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.InternalServerError>();
        }

        [TestMethod]
        public async Task Create_UnknownError_ReturnsBadGatewayResult()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                        {
                            Body = new VisionResponseBody<PatientConfigurationResponse>
                            {
                                VisionResponse = new VisionResponse<PatientConfigurationResponse>
                                {
                                    ServiceHeader = new ServiceHeaderResponse
                                    {
                                        Outcome = new Outcome
                                        {
                                            Successful = "false",
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

            var systemUnderTest = CreateVisionSessionService();

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode, _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        [TestMethod]
        public async Task Create_WhenNoMatchedError_ReturnsBadGateway()
        {
            // Arrange
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                        {
                            Body = new VisionResponseBody<PatientConfigurationResponse>
                            {
                                Fault = new Fault(), // fault but no code
                            },
                        },
                    }));

            var systemUnderTest = CreateVisionSessionService();

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode, _nhsNumber);

            // Assert
            result.Should().BeAssignableTo<GpSessionCreateResult.BadGateway>();
        }

        private VisionSessionService CreateVisionSessionService()
        {
            return new VisionSessionService(
                _mockVisionClient.Object,
                _mockLogger,
                new VisionTokenValidationService(new Mock<ILogger<VisionTokenValidationService>>().Object));
        }
    }
}
