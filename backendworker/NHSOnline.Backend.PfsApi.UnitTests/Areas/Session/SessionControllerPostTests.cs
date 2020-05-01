using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.PfsApi.UnitTests.Audit;
using NHSOnline.Backend.PfsApi.UserInfo;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support.Session;
using UnitTestHelper;
using ConfigurationSettings = NHSOnline.Backend.Support.Settings.ConfigurationSettings;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionControllerPostTests
    {
        private const string Name = "Name";
        private const int SessionTimeoutMinutes = 10;
        private const int SessionTimeoutSeconds = 600;
        private const string ApiSessionId = "ApiSessionId";
        private const string ServiceDeskReference = "ServiceDeskReference";
        private const string AccessToken = "AccessToken";
        private const string CsrfRequestToken = "dskhfakserhhvjcgbfdsh";

        private Mock<ICitizenIdSessionService> _mockCitizenIdSessionService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<ILogger<SessionController>> _mockSessionControllerLogger;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IIm1CacheService> _mockIm1CacheService;
        private Mock<IOdsCodeMassager> _mockOdsCodeMassager;
        private Mock<IServiceJourneyRulesService> _mockServiceJourneyRulesService;
        private Mock<IErrorReferenceGenerator> _mockErrorReferenceGenerator;
        private Mock<IGpSessionManager> _mockGpSessionManager;
        private Mock<HttpContext> _mockHttpContext;
        private Mock<IAntiforgery> _mockAntiforgery;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<IAuthenticationService> _mockAuthenticationService;

        private EmisConnectionToken _connectionToken;
        private UserSessionRequest _userSessionRequest;
        private Auth.CitizenId.Models.UserInfo _userInfo;
        private UserProfile _userProfile;
        private EmisUserSession _emisUserSession;
        private CitizenIdUserSession _citizenIdUserSession;
        private CitizenIdSessionResult _citizenIdSessionResult;
        private ServiceJourneyRulesResponse _serviceJourneyRulesResponse;
        private SessionConfigurationSettings _sessionConfigSettings;
        private P9UserSession _userSession;
        private ServiceJourneyRulesConfigResult _serviceJourneyRulesConfigResult;
        private ConfigurationSettings _configurationSettings;

        private ServiceProvider _serviceProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            InitializeData();
            InitializeMocks();
            InitializeServiceProvider();
        }

        private void InitializeData()
        {
            _connectionToken = new EmisConnectionToken();

            _userSessionRequest = new UserSessionRequest
            {
                AuthCode = "AuthCode",
                CodeVerifier = "CodeVerifier",
                RedirectUrl = "http://RedirectUrl/"
            };

            _userInfo = new Auth.CitizenId.Models.UserInfo
            {
                Birthdate = "1980-01-02",
                Im1ConnectionToken = _connectionToken.SerializeJson(),
                NhsNumber = "012 345 6789",
                GpIntegrationCredentials = { OdsCode = "OdsCode" }
            };
            _userProfile = new UserProfile(_userInfo, AccessToken);

            _emisUserSession = new EmisUserSession
            {
                OdsCode = _userProfile.OdsCode,
                NhsNumber = _userProfile.NhsNumber,
                Name = Name
            };

            _citizenIdUserSession = new CitizenIdUserSession
            {
                AccessToken = AccessToken,
                ProofLevel = ProofLevel.P9,
                OdsCode = _userProfile.OdsCode
            };

            _citizenIdSessionResult = new CitizenIdSessionResult
            {
                StatusCode = (int) HttpStatusCode.OK,
                DateOfBirth = new DateTime(1980, 01, 02, 0, 0, 0, DateTimeKind.Utc),
                Im1ConnectionToken = _userProfile.Im1ConnectionToken,
                NhsNumber = _userProfile.NhsNumber,
                Session = _citizenIdUserSession
            };

            _serviceJourneyRulesResponse = new ServiceJourneyRulesResponse { Journeys = new Journeys { Supplier = Supplier.Emis } };
            _serviceJourneyRulesConfigResult = new ServiceJourneyRulesConfigResult.Success(_serviceJourneyRulesResponse);

            _sessionConfigSettings = new SessionConfigurationSettings(false);

            _userSession = new P9UserSession(CsrfRequestToken, _citizenIdUserSession, _emisUserSession, string.Empty);

            _configurationSettings = new ConfigurationSettings
            {
                DefaultSessionExpiryMinutes = SessionTimeoutMinutes,
            };
        }

        private void InitializeMocks()
        {
            _mockCitizenIdSessionService = new Mock<ICitizenIdSessionService>();
            _mockCitizenIdSessionService
                .Setup(x => x.Create(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier,
                    new Uri(_userSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(_citizenIdSessionResult));

            _mockGpSystem = new Mock<IGpSystem>();
            _mockGpSystem.SetupGet(x => x.Supplier).Returns(Supplier.Emis);

            _mockGpSystemFactory = new Mock<IGpSystemFactory>();
            _mockGpSystemFactory.Setup(x => x.CreateGpSystem(Supplier.Emis)).Returns(_mockGpSystem.Object);

            _mockSessionControllerLogger = new Mock<ILogger<SessionController>>();
            _mockSessionControllerLogger.SetupLogger(LogLevel.Information, $"NhsNumber={_userProfile.NhsNumber}", null);

            _mockAuditor = new Mock<IAuditor>();

            _mockIm1CacheService = new Mock<IIm1CacheService>();
            _mockIm1CacheService
                .Setup(x => x.DeleteIm1ConnectionToken(_connectionToken.Im1CacheKey))
                .Returns(Task.FromResult(true)).Verifiable();

            _mockOdsCodeMassager = new Mock<IOdsCodeMassager>();
            _mockOdsCodeMassager.Setup(x => x.CheckOdsCode(_userProfile.OdsCode)).Returns(_userProfile.OdsCode);

            _mockServiceJourneyRulesService = new Mock<IServiceJourneyRulesService>();
            _mockServiceJourneyRulesService
                .Setup(x => x.GetServiceJourneyRulesForOdsCode(_userProfile.OdsCode))
                .Returns(Task.FromResult(_serviceJourneyRulesConfigResult));

            _mockErrorReferenceGenerator = new Mock<IErrorReferenceGenerator>();

            _mockGpSessionManager = new Mock<IGpSessionManager>();

            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpContext.Setup(x => x.RequestServices).Returns(() => _serviceProvider);

            _mockAntiforgery = new Mock<IAntiforgery>();
            _mockAntiforgery
                .Setup(x => x.GetTokens(_mockHttpContext.Object))
                .Returns(new AntiforgeryTokenSet(CsrfRequestToken, "cookieToken", "formFieldName", "headerName"));

            _mockSessionCacheService = new Mock<ISessionCacheService>();
            _mockSessionCacheService
                .Setup(x => x.CreateUserSession(It.IsAny<UserSession>()))
                .Callback<UserSession>(userSession => userSession.Key = ApiSessionId)
                .Returns(Task.FromResult(ApiSessionId));

            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockAuthenticationService
                .Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));
        }

        private void InitializeServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<SessionController>()
                .AddSingleton(_mockCitizenIdSessionService.Object)
                .AddSingleton(_mockGpSystemFactory.Object)
                .AddSingleton(_configurationSettings)
                .AddSingleton(_mockSessionControllerLogger.Object)
                .AddSingleton(_mockAuditor.Object)
                .AddSingleton(_mockIm1CacheService.Object)
                .AddSingleton(_mockOdsCodeMassager.Object)
                .AddSingleton(_mockServiceJourneyRulesService.Object)
                .AddSingleton(_mockErrorReferenceGenerator.Object)
                .AddSingleton(new Mock<IUserInfoService>().Object)
                .AddSingleton(_mockGpSessionManager.Object)
                .AddSingleton(_mockAntiforgery.Object)
                .AddSingleton(_mockSessionCacheService.Object)
                .AddSingleton(_mockAuthenticationService.Object)
                .AddSingleton(new Mock<ILogger<SessionCreator>>().Object)
                .AddSingleton(new Mock<ILogger<SessionCreatorCitizenIdService>>().Object)
                .AddSingleton(new Mock<ILogger<SessionCreatorServiceJourneyRuleService>>().Object)
                .AddSingleton(new Mock<ILogger<UserSessionManager>>().Object)
                .AddSingleton(new Mock<ILogger<P5UserSessionCreator>>().Object)
                .AddSingleton(new Mock<ILogger<P9UserSessionCreator>>().Object);

            new PfsApi.Session.ServiceConfigurationModule().ConfigureServices(serviceCollection, null);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        public async Task Post_CIDUserProfileCallFails_ReturnsBadRequest()
        {
            // Arrange
            _mockCitizenIdSessionService
                .Setup(x =>
                    x.Create(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier,
                        new Uri(_userSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(new CitizenIdSessionResult
                {
                    StatusCode = (int) HttpStatusCode.BadRequest
                }))
                .Verifiable();

            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.Is<ErrorTypes>(
                        et =>
                            et.Category == ErrorCategory.Login &&
                            et.StatusCode == StatusCodes.Status400BadRequest &&
                            et.SourceApi == SourceApi.None)))
                .Returns(ServiceDeskReference)
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = ServiceDeskReference,
            };

            // Act
            var result = await CreateSystemUnderTest().Post(_userSessionRequest);

            // Assert
            _mockCitizenIdSessionService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_CIDUserProfileCallFails_WithBadGateway_ReturnsBadGateway()
        {
            // Arrange
            _mockCitizenIdSessionService
                .Setup(x =>
                    x.Create(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier,
                       new Uri(_userSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(new CitizenIdSessionResult
                {
                    StatusCode = (int) HttpStatusCode.BadGateway
                }))
                .Verifiable();

            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Login,
                    StatusCodes.Status502BadGateway, SourceApi.NhsLogin))
                .Returns(ServiceDeskReference)
                .Verifiable();

            // Act
            var result = await CreateSystemUnderTest().Post(_userSessionRequest);

            // Assert
            _mockCitizenIdSessionService.Verify();
            result.Should().BeAssignableTo<ObjectResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAuditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_Returns464OdsCodeNotSupported()
        {
            // Arrange
            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.IsAny<ErrorTypes.LoginOdsCodeNotFoundOrNotSupported>()))
                .Returns(ServiceDeskReference)
                .Verifiable();

            _serviceJourneyRulesConfigResult =
                new ServiceJourneyRulesConfigResult.Success(new ServiceJourneyRulesResponse
                {
                    Journeys = new Journeys { Supplier = Supplier.Unknown }
                });

            _mockServiceJourneyRulesService
                .Setup(x => x.GetServiceJourneyRulesForOdsCode(_userProfile.OdsCode))
                .Returns(Task.FromResult(_serviceJourneyRulesConfigResult))
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = ServiceDeskReference
            };

            var auditStub = ArrangeAudit();

            // Act
            var result = await CreateSystemUnderTest().Post(_userSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should()
                    .Be(Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
                auditStub.AccessTokenString.Should().Be(_citizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(_userProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Unknown);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Failed to determine the GP system based on ODS code 'OdsCode'");
            }

            _mockSessionControllerLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GpSessionManagerReturnInvalidConnectionToken_ReturnsForbidden()
        {
            // Arrange
            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginForbidden>()))
                .Returns(ServiceDeskReference)
                .Verifiable();
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = ServiceDeskReference
            };

            var auditStub = ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.InvalidConnectionToken());

            // Act
            var result = await CreateSystemUnderTest().Post(_userSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
                auditStub.AccessTokenString.Should().Be(_citizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(_userProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Creating the session failed: Invalid connection token");
            }

            _mockAuditor.Verify();
            _mockSessionControllerLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GpSessoinManagerReturnsForbidden_ReturnsForbidden()
        {
            // Arrange
            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.Is<ErrorTypes>(
                        et =>
                            et.Category == ErrorCategory.Login &&
                            et.StatusCode == StatusCodes.Status403Forbidden &&
                            et.SourceApi == SourceApi.None)))
                .Returns(ServiceDeskReference)
                .Verifiable();

            _userSession.Key = "123";

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = ServiceDeskReference
            };

            var auditStub = ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Forbidden("Message"));

            // Act
            var result = await CreateSystemUnderTest().Post(_userSessionRequest);

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
                auditStub.AccessTokenString.Should().Be(_citizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(_userProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Creating the session failed: Message");
            }

            _mockAuditor.Verify();
            _mockSessionControllerLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GpSupplierSessionCreateFails_Returns502BadGateway()
        {
            // Arrange
            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.Is<ErrorTypes>(
                        et =>
                            et.Category == ErrorCategory.Login &&
                            et.StatusCode == StatusCodes.Status502BadGateway &&
                            et.SourceApi == SourceApi.Emis)))
                .Returns(ServiceDeskReference)
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = ServiceDeskReference
            };

            var auditStub = ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.BadGateway("Message"));

            // Act
            var result = await CreateSystemUnderTest().Post(_userSessionRequest);

            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
                auditStub.AccessTokenString.Should().Be(_citizenIdUserSession.AccessToken);
                auditStub.NhsNumber.Should().Be(_userProfile.NhsNumber);
                auditStub.Supplier.Should().Be(Supplier.Emis);
                auditStub.Operation.Should().Be("GP_Session_Create");
                auditStub.Details.Should().Be("Attempting to create Session");
                auditStub.ResponseDetails.Should().Be("Creating the session failed: Message");
            }

            _mockSessionControllerLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GetServiceJourneyRulesForOdsReturnsNotFound_Returns464InternalServerError()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _emisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginOdsCodeNotFoundOrNotSupported>()))
                .Returns(ServiceDeskReference)
                .Verifiable();
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = ServiceDeskReference
            };
            _serviceJourneyRulesConfigResult =
                new ServiceJourneyRulesConfigResult.NotFound();

            _mockServiceJourneyRulesService
                .Setup(x =>
                    x.GetServiceJourneyRulesForOdsCode(_userProfile.OdsCode))
                .Returns(Task.FromResult(_serviceJourneyRulesConfigResult))
                .Verifiable();

            _mockAuditor.Setup(x => x.Audit()).Returns(new AuditBuilderStub());

            // Act
            var result = await CreateSystemUnderTest().Post(_userSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockServiceJourneyRulesService.Verify();
            _mockSessionControllerLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GetServiceJourneyRulesForOdsReturnsInternalServerError_Returns500InternalServerError()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _emisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            _mockErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginServiceJourneyRulesOtherError>()))
                .Returns(ServiceDeskReference)
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = ServiceDeskReference
            };

            _serviceJourneyRulesConfigResult =
                new ServiceJourneyRulesConfigResult.InternalServerError();

            _mockServiceJourneyRulesService
                .Setup(x =>
                    x.GetServiceJourneyRulesForOdsCode(_userProfile.OdsCode))
                .Returns(Task.FromResult(_serviceJourneyRulesConfigResult))
                .Verifiable();

            _mockAuditor.Setup(x => x.Audit()).Returns(new AuditBuilderStub());

            // Act
            var result = await CreateSystemUnderTest().Post(_userSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockServiceJourneyRulesService.Verify();
            _mockSessionControllerLogger.Verify();
        }


        [TestMethod]
        public async Task Post_HappyPath_ReturnsUsersSessionResponse()
        {
            _sessionConfigSettings.ProxyEnabled = true;

            _emisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };

            _userSession.Key = "123";

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(_userSession.GpUserSession));
            ArrangeAudit();

            // Act
            var result = await CreateSystemUnderTest().Post(_userSessionRequest);

            // Assert
            var expectedUserSessionResponse = new PostUserSessionResponse
            {
                Name = Name,
                SessionTimeout = SessionTimeoutSeconds,
                OdsCode = _userProfile.OdsCode,
                DateOfBirth = _citizenIdUserSession.DateOfBirth,
                NhsNumber = _userProfile.NhsNumber,
                AccessToken = _userProfile.AccessToken,
                ServiceJourneyRules = _serviceJourneyRulesResponse
            };

            using (new AssertionScope())
            {
                var createdResultValue = result.Should().BeAssignableTo<CreatedResult>().Subject.Value;
                var actualUserSessionResponse = createdResultValue.Should().BeAssignableTo<PostUserSessionResponse>().Subject;
                actualUserSessionResponse.Name.Should().Be(expectedUserSessionResponse.Name);
                actualUserSessionResponse.SessionTimeout.Should().Be(expectedUserSessionResponse.SessionTimeout);
                actualUserSessionResponse.Token.Should().Be(CsrfRequestToken);
                actualUserSessionResponse.OdsCode.Should().Be(expectedUserSessionResponse.OdsCode);
                actualUserSessionResponse.DateOfBirth.Should().Be(expectedUserSessionResponse.DateOfBirth);
                actualUserSessionResponse.NhsNumber.Should().Be(expectedUserSessionResponse.NhsNumber);
                actualUserSessionResponse.AccessToken.Should().Be(expectedUserSessionResponse.AccessToken);
                actualUserSessionResponse.ServiceJourneyRules.Should().BeEquivalentTo(expectedUserSessionResponse.ServiceJourneyRules);
            }
        }

        [TestMethod]
        public async Task Post_Im1ConnectionTokenHasCacheKey_AttemptsToDeleteFromCache()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _emisUserSession.ProxyPatients = new List<EmisProxyUserSession>
            {
                new EmisProxyUserSession()
            };
            _userSession.Key = "123";

            _mockIm1CacheService
                .Setup(x => x.DeleteIm1ConnectionToken(_connectionToken.Im1CacheKey))
                .Returns(Task.FromResult(true));

            ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(_userSession.GpUserSession));

            // Act
            await CreateSystemUnderTest().Post(_userSessionRequest);

            // Assert
            _mockIm1CacheService.Verify();
        }

        [TestMethod]
        public async Task Post_Im1ConnectionTokenIsAGuid_DoesNotAttemptToDeleteFromCache()
        {
            // Arrange
            _userInfo.Im1ConnectionToken  = "0E5DD1C4-C519-4EF5-9B0F-624357F6F26F";

            _mockCitizenIdSessionService
                .Setup(x => x.Create(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier, new Uri(_userSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(_citizenIdSessionResult));

            _mockIm1CacheService.Reset();
            _mockIm1CacheService
                .Setup(x => x.DeleteIm1ConnectionToken(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            ArrangeAudit();

            ArrangeGpSessionManagerCreateSession(new GpSessionCreateResult.Success(_userSession.GpUserSession));

            // Act
            await CreateSystemUnderTest().Post(_userSessionRequest);

            // Assert
            _mockIm1CacheService.Verify(x => x.DeleteIm1ConnectionToken(It.IsAny<string>()), Times.Never);
        }

        private void ArrangeGpSessionManagerCreateSession(GpSessionCreateResult returnResult)
        {
            _mockGpSessionManager
                .Setup(
                    x => x.CreateSession(
                        It.Is<IGpSessionCreateArgs>(p
                            => ReferenceEquals(p.GpSystem, _mockGpSystem.Object) &&
                               p.NhsNumber.Equals(_citizenIdSessionResult.NhsNumber, StringComparison.Ordinal) &&
                               p.OdsCode.Equals(_citizenIdSessionResult.Session.OdsCode, StringComparison.Ordinal) &&
                               p.Im1ConnectionToken.Equals(_citizenIdSessionResult.Im1ConnectionToken, StringComparison.Ordinal))))
                .ReturnsAsync(returnResult);
        }

        private AuditBuilderStub ArrangeAudit()
        {
            var auditBuilderStub = new AuditBuilderStub();
            _mockAuditor.Setup(x => x.Audit()).Returns(auditBuilderStub);
            return auditBuilderStub;
        }

        private SessionController CreateSystemUnderTest()
        {
            var systemUnderTest = _serviceProvider.GetRequiredService<SessionController>();

            systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            };

            return systemUnderTest;
        }
    }
}