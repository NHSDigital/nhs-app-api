using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support;
using Im1ConnectionErrorCodes = NHSOnline.Backend.GpSystems.Im1Connection.Im1ConnectionErrorCodes;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Im1Connection
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
        private Mock<IIm1CacheService> _im1CacheService;
        private Mock<IIm1CacheKeyGenerator> _im1CacheKeyGenerator;
        private VisionIm1ConnectionService _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockVisionClient = _fixture.Freeze<Mock<IVisionClient>>();
            _im1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();
            _im1CacheKeyGenerator = _fixture.Freeze<Mock<IIm1CacheKeyGenerator>>();
            _systemUnderTest = _fixture.Create<VisionIm1ConnectionService>();
        }

        [TestMethod]
        public async Task Verify_ReturnsAConnection_WhenRequested()
        {
            // Arrange
            var patientConfiguration = _fixture.Create<PatientConfiguration>();

            var expectedNhsNumbers =
                patientConfiguration.Account.PatientNumbers.Select(x => new PatientNhsNumber { NhsNumber = x.Number });

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
                                        Configuration = patientConfiguration,
                                    },
                                },
                            },
                        },
                    }));
            
            // Act
            var result = await _systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            var successResult = result.Should().BeAssignableTo<Im1ConnectionVerifyResult.Success>()
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

            // Act
            var result = await _systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be( Im1ConnectionErrorCodes.InternalCode.InvalidRequest);
        }

        [TestMethod]
        public async Task Verify_InvalidUserCredentials_ReturnsInvalidLinkageDetailsError()
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

            // Act
            var result = await _systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be( Im1ConnectionErrorCodes.InternalCode.InvalidLinkageDetails);
        }

        [TestMethod]
        public async Task Verify_InvalidSecurityHeader_ReturnsInvalidSecurityError()
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

            // Act
            var result = await _systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be( Im1ConnectionErrorCodes.InternalCode.InvalidSecurity);
        }

        [TestMethod]
        public async Task Verify_UnknownError_ReturnsConnectionToServiceFailedError()
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

            // Act
            var result = await _systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode>()
                .Subject.ErrorCode.Should().Be( Im1ConnectionErrorCodes.InternalCode.UnknownError);
        }

        [TestMethod]
        public async Task Verify_WhenNoMatchedError_ReturnsUnmappedErrorWithStatusCode()
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

            // Act
            var result = await _systemUnderTest.Verify(DefaultConnectionToken, DefaultOdsCode);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode>();
        }
        
        [TestMethod]
        public async Task Register_WhenConnectionTokenCached_SuccessfulRegister_ObtainNHSnumber()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();
            var apiKey = _fixture.Create<string>();

            var connectionToken = new VisionConnectionToken
            {
                RosuAccountId = AccountId,
                ApiKey = apiKey,
            };

            var patientConfiguration = _fixture.Create<PatientConfiguration>();

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
                                        Configuration = patientConfiguration,
                                    },
                                },
                            },
                        },
                    }));

            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };

            _im1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(request.AccountId, request.OdsCode, request.LinkageKey))
                .Returns(cacheKey);
            _im1CacheService
                .Setup(x => x.GetIm1ConnectionToken<VisionConnectionToken>(cacheKey))
                .Returns(Task.FromResult(Option.Some(connectionToken)))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.Success>();

            _mockVisionClient.Verify(x =>
                    x.PostLinkAccount(It.IsAny<string>(), It.IsAny<PatientIm1ConnectionRequest>(), It.IsAny<string>()),
                Times.Never);
            _im1CacheService.Verify(
                x => x.SaveIm1ConnectionToken(It.IsAny<string>(), It.IsAny<VisionConnectionToken>()), Times.Never);
        }

        [TestMethod]
        public async Task Register_WhenConnectionTokenNotCached_SuccessfulRegister_ObtainNHSnumber()
        {
            // Arrange
            var apiKey = _fixture.Create<string>();
            var cacheKey = _fixture.Create<string>();

            var serviceContentAuthenticationRef = new ServiceContentAuthenticationRef
            {
                ApiToken = apiKey,
            };

            _mockVisionClient.Setup(x =>
                    x.PostLinkAccount(It.IsAny<string>(), It.IsAny<PatientIm1ConnectionRequest>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<ServiceContentRegisterResponse>(HttpStatusCode.OK)
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
                                        Configuration = patientConfiguration,
                                    },
                                },
                            },
                        },
                    }));

            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };

            _im1CacheKeyGenerator.Setup(x => x.GenerateCacheKey(request.AccountId, request.OdsCode, request.LinkageKey))
                .Returns(cacheKey).Verifiable();
            _im1CacheService
                .Setup(x => x.GetIm1ConnectionToken<VisionConnectionToken>(cacheKey))
                .Returns(Task.FromResult(Option.None<VisionConnectionToken>()))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.Success>();

            _im1CacheKeyGenerator.VerifyAll();
            _im1CacheService.Verify(x => x.SaveIm1ConnectionToken(
                cacheKey,
                It.Is<VisionConnectionToken>(
                    con => string.Equals(con.ApiKey, apiKey, StringComparison.Ordinal)
                           && string.Equals(con.RosuAccountId, AccountId, StringComparison.Ordinal)
                           && string.Equals(con.Im1CacheKey, cacheKey, StringComparison.Ordinal))));
        }

        [TestMethod]
        public async Task Register_UserAccountLocked()
        {
            // Arrange
            const string errorCode = "-15";
            const string errorDescription =
                "Record currently unavailable - please try again later or contact your Practice: VOSUsers record is locked, is patient selected in registration?";
            _mockVisionClient = PostLinkMockError(errorCode, errorDescription);
            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>();
            var errorResult = (Im1ConnectionRegisterResult.ErrorCase)result;
            errorResult.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UserAccountDisabled);
        }

        [TestMethod]
        public async Task Register_UserAlreadyRegistered()
        {
            // Arrange
            const string errorCode = "-2";
            const string errorDescription = "User has already been registered";
            _mockVisionClient = PostLinkMockError(errorCode, errorDescription);
            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UserAlreadyLinked);
        }

        [TestMethod]
        public async Task Register_WhenPostLinkAccount_ThrowsSocketException_AccountAlreadyExists()
        {
            // Arrange
            var cacheKey = _fixture.Create<string>();

            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };

            _mockVisionClient.Setup(x =>
                    x.PostLinkAccount(It.IsAny<string>(), It.IsAny<PatientIm1ConnectionRequest>(), It.IsAny<string>()))
                .Throws<SocketException>();

            _im1CacheKeyGenerator
                .Setup(x => x.GenerateCacheKey(request.AccountId, request.OdsCode, request.LinkageKey))
                .Returns(cacheKey);

            _im1CacheService
                .Setup(x => x.GetIm1ConnectionToken<VisionConnectionToken>(cacheKey))
                .Returns(Task.FromResult(Option.None<VisionConnectionToken>()));

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.UserAlreadyLinked);
        }

        [TestMethod]
        public async Task Register_IncorrectDetailsProvided()
        {
            // Arrange
            const string errorCode = "-33";
            const string errorDescription = "No Match: couldn't link account with detail provided";
            _mockVisionClient = PostLinkMockError(errorCode, errorDescription);
            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.NoUserFoundForLinkageDetails);
        }

        [TestMethod]
        public async Task Register_IncorrectParametersProvided()
        {
            // Arrange
            const string errorCode = "-31";
            const string errorDescription = "Invalid parameter provided";
            _mockVisionClient = PostLinkMockError(errorCode, errorDescription);
            var request = new PatientIm1ConnectionRequest
            {
                OdsCode = DefaultOdsCode,
                AccountId = AccountId,
                DateOfBirth = _dob,
                LinkageKey = LinkageKey,
                Surname = Surname
            };

            // Act
            var result = await _systemUnderTest.Register(request);

            // Assert
            result.Should().BeAssignableTo<Im1ConnectionRegisterResult.ErrorCase>()
                .Subject.ErrorCode.Should().Be(Im1ConnectionErrorCodes.InternalCode.InvalidLinkageDetails);
        }

        private Mock<IVisionClient> PostLinkMockError(string errorCode, string errorDescription)
        {
            _mockVisionClient.Setup(x =>
                    x.PostLinkAccount(It.IsAny<string>(), It.IsAny<PatientIm1ConnectionRequest>(), It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new VisionPFSClient.VisionApiObjectResponse<ServiceContentRegisterResponse>(HttpStatusCode.OK)
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