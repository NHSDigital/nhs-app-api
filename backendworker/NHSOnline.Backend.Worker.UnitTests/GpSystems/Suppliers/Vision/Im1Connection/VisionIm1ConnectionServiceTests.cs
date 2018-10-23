using System;
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
        private const string AccountId = "account_id";
        private const string Surname = "surname";
        private const string LinkageKey = "key";
        
        private readonly DateTime _dob = DateTime.Now;
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
                    new VisionClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                        {
                            Body = new VisionResponseBody<PatientConfigurationResponse>
                            {
                                VisionResponse = new VisionResponse<PatientConfigurationResponse>
                                {
                                    ServiceContent = new PatientConfigurationResponse
                                    {
                                        Configuration = patientConfiguration,
                                    },
                                },
                            },
                        },
                    }));

            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);

            var result = await systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            var successResult = result.Should().BeAssignableTo<Im1ConnectionVerifyResult.SuccessfullyVerified>()
                .Subject;

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
                    new VisionClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
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
                    new VisionClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
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
                    new VisionClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
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
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
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
                    new VisionClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                        {
                            Body = new VisionResponseBody<PatientConfigurationResponse>
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
        
        [TestMethod]
        public async Task Register_SuccessfulRegister_ObtainNHSnumber()
        {
            var serviceContentAuthenticationRef = _fixture.Create<ServiceContentAuthenticationRef>();
            _mockVisionClient.Setup(x =>
                    x.PostLinkAccount(It.IsAny<string>(), It.IsAny<PatientIm1ConnectionRequest>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionClient.VisionApiObjectResponse<ServiceContentRegisterResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<ServiceContentRegisterResponse>
                        {
                            Body = new VisionResponseBody<ServiceContentRegisterResponse>
                            {
                                VisionResponse = new VisionResponse<ServiceContentRegisterResponse>
                                {
                                    ServiceContent = new ServiceContentRegisterResponse
                                    {
                                        AuthenticationRef = serviceContentAuthenticationRef,
                                    },
                                },
                            },
                        },
                    }));
            var patientConfiguration = _fixture.Create<PatientConfiguration>();
            _mockVisionClient.Setup(x =>
                    x.GetConfiguration(It.IsAny<VisionConnectionToken>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionClient.VisionApiObjectResponse<PatientConfigurationResponse>(HttpStatusCode.OK)
                    {
                        RawResponse = new VisionResponseEnvelope<PatientConfigurationResponse>
                        {
                            Body = new VisionResponseBody<PatientConfigurationResponse>
                            {
                                VisionResponse = new VisionResponse<PatientConfigurationResponse>
                                {
                                    ServiceContent = new PatientConfigurationResponse
                                    {
                                        Configuration = patientConfiguration,
                                    },
                                },
                            },
                        },
                    }));
            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);
            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth  = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };
            var result = await systemUnderTest.Register(request);
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SuccessfullyRegistered>();
        }
        
        [TestMethod]
        public async Task Register_UserAccountLocked()
        {
            const string errorCode = "-15";
            const string errorDescription = "Record currently unavailable - please try again later or contact your Practice: VOSUsers record is locked, is patient selected in registration?";
            _mockVisionClient = PostLinkMockError(errorCode, errorDescription);
            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);
            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth  = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };
            var result = await systemUnderTest.Register(request);
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.SupplierSystemUnavailable>();
        }
        
        [TestMethod]
        public async Task Register_UserAlreadyRegistered()
        {
            const string errorCode = "-2";
            const string errorDescription = "User has already been registered";
            _mockVisionClient = PostLinkMockError(errorCode, errorDescription);
            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);
            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth  = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };
            var result = await systemUnderTest.Register(request);
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.AccountAlreadyExists>();
        }
        
        [TestMethod]
        public async Task Register_IncorrectDetailsProvided()
        {
            const string errorCode = "-33";
            const string errorDescription = "No Match: couldn't link account with detail provided";
            _mockVisionClient = PostLinkMockError(errorCode, errorDescription);
            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);
            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth  = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };
            var result = await systemUnderTest.Register(request);
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.NotFound>();
        }

        [TestMethod]
        public async Task Register_IncorrectParametersProvided()
        {
            const string errorCode = "-31";
            const string errorDescription = "Invalid parameter provided";
            _mockVisionClient = PostLinkMockError(errorCode, errorDescription);
            var systemUnderTest = new VisionIm1ConnectionService(_mockVisionClient.Object, _logger);
            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth  = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };
            var result = await systemUnderTest.Register(request);
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.BadRequest>();
        }
        
        private Mock<IVisionClient> PostLinkMockError(string errorCode, string errorDescription)
        {
            _mockVisionClient.Setup(x =>
                    x.PostLinkAccount(It.IsAny<string>(), It.IsAny<PatientIm1ConnectionRequest>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionClient.VisionApiObjectResponse<ServiceContentRegisterResponse>(HttpStatusCode.OK)
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
                                                Description = errorDescription
                                            }
                                        }
                                    }
                                },
                            },
                        },
                    }));
            return _mockVisionClient;
        }
    }
}
