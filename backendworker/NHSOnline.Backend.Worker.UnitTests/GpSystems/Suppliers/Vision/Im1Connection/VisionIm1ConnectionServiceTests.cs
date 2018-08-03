using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.Im1Connection
{
    [TestClass]
    public class VisionIm1ConnectionServiceTests
    {
        private const string DefaultConnectionToken = "{\"rosuAccountId\":\"account_id\",\"apiKey\":\"key\"}";
        private const string DefaultOdsCode = "token";

        private IFixture _fixture;
        private Mock<IVisionClient> _mockVisionClient;
        private ILogger<VisionIm1ConnectionService> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _logger = Mock.Of<ILogger<VisionIm1ConnectionService>>();
        }

        [TestMethod]
        public async Task Verify_ReturnsAConnection_WhenRequested()
        {
            var patientConfiguration = _fixture.Create<PatientConfiguration>();

            var expectedNhsNumbers =
                patientConfiguration.Account.PatientNumbers.Select(x => new PatientNhsNumber { NhsNumber = x.Number });

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
                                        Payload = patientConfiguration,
                                    },
                                },
                            },
                        },
                    }));

            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>();

            var successResult = result as Im1ConnectionVerifyResult.SuccessfullyVerified;

            successResult.Response.ConnectionToken.Should().Be(DefaultConnectionToken);
            successResult.Response.NhsNumbers.Should().BeEquivalentTo(expectedNhsNumbers);
        }

        [TestMethod]
        public async Task Verify_InvalidRequest_ReturnsInvalidRequestResult()
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

            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);

            // Act
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.InvalidRequest>();
        }

        [TestMethod]
        public async Task Verify_InvalidUserCredentials_ReturnsInvalidUserCredentialsResult()
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
                                            Successful = bool.FalseString.ToLowerInvariant(),
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

            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);

            // Act
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.InvalidUserCredentials>();
        }

        [TestMethod]
        public async Task Verify_InvalidSecurityHeader_ReturnsErrorProcessingSecurityHeaderResult()
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

            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);

            // Act
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader>();
        }

        [TestMethod]
        public async Task Verify_UnknownError_ReturnsUnknownErrorResult()
        {
            // Arrange
            var patientConfiguration = _fixture.Create<PatientConfiguration>();

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
                                            Successful = bool.FalseString.ToLowerInvariant(),
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

            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);

            // Act
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.UnknownError>();
        }

        [TestMethod]
        public async Task Verify_WhenNoMatchedError_ReturnsSupplierSystemUnavailableResult()
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

            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);

            // Act
            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SupplierSystemUnavailable>();
        }
    }
}
