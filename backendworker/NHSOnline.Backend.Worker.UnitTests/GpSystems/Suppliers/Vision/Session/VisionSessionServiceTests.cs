using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Worker.Settings;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Session
{
    [TestClass]
    public class VisionSessionServiceTests
    {
        private const string DefaultConnectionToken = "{\"rosuAccountId\":\"account_id\",\"apiKey\":\"key\"}";
        private const string DefaultOdsCode = "token";
        private IOptions<ConfigurationSettings> _options;
        private IFixture _fixture;
        private Mock<IVisionClient> _mockVisionClient;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _options = Options.Create(new ConfigurationSettings());
        }

        [TestMethod]
        public async Task Create_ValidRequest_ReturnsSuccessfullyCreatedResult()
        {
            // Arrange
            var accountName = _fixture.Create<string>();
            var patientNumber = _fixture.Create<PatientNumber>();
            patientNumber.NumberType = "NHS";

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
                                    ServiceContent = new ServiceContentResponse<PatientConfiguration>
                                    {
                                        Payload = new PatientConfiguration
                                        {
                                            Account = new Account
                                            {
                                                Name = accountName,
                                                PatientNumbers = new List<PatientNumber> {patientNumber}
                                            }
                                        }
                                    }
                                }
                            },
                        },
                    }));
            
            var systemUnderTest = new VisionSessionService(_mockVisionClient.Object, _options);

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<SessionCreateResult.SuccessfullyCreated>();

            var successResult = result as SessionCreateResult.SuccessfullyCreated;
            successResult.Name.Should().Equals(accountName);
        }

        [TestMethod]
        public async Task Create_InvalidRequest_ReturnsInvalidRequestResult()
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

            var systemUnderTest = new VisionSessionService(_mockVisionClient.Object, _options);

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<SessionCreateResult.InvalidRequest>();
        }
        
        [TestMethod]
        public async Task Create_InvalidUserCredentials_ReturnsInvalidUserCredentialsResult()
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

            var systemUnderTest = new VisionSessionService(_mockVisionClient.Object, _options);

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<SessionCreateResult.InvalidUserCredentials>();
        }

        [TestMethod]
        public async Task Create_InvalidSecurityHeader_ReturnsErrorProcessingSecurityHeaderResult()
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

            var systemUnderTest = new VisionSessionService(_mockVisionClient.Object, _options);

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<SessionCreateResult.ErrorProcessingSecurityHeader>();
        }

        [TestMethod]
        public async Task Create_UnknownError_ReturnsUnknownErrorResult()
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

            var systemUnderTest = new VisionSessionService(_mockVisionClient.Object, _options);

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<SessionCreateResult.UnknownError>();
        }

        [TestMethod]
        public async Task Create_WhenNoMatchedError_ReturnsSupplierSystemUnavailableResult()
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

            var systemUnderTest = new VisionSessionService(_mockVisionClient.Object, _options);

            // Act
            var result = await systemUnderTest.Create(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<SessionCreateResult.SupplierSystemUnavailable>();
        }
    }
}