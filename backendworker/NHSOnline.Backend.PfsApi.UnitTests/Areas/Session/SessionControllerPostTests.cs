using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support.Settings;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionControllerPostTests
    {
        private SessionController _systemUnderTest;
        private IFixture _fixture;
        private Mock<ICitizenIdSessionService> _mockCitizenIdSessionService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IOdsCodeLookup> _mockOdsCodeLookup;
        private Mock<IErrorReferenceGenerator> _mockServiceDeskErrorReferenceGenerator;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<ISessionService> _mockSessionService;
        private Mock<ITokenValidationService> _mockTokenValidationService;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private ConfigurationSettings _configurationSettings;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IIm1CacheService> _mockIm1CacheService;
        private Mock<ISessionMapper> _mockSessionMapper;
        private Mock<ILogger<SessionController>> _mockLogger;
        private Mock<IOdsCodeMassager> _mockOdsCodeMassager;
        private Mock<IServiceJourneyRulesService> _mockServiceJourneyRulesService;

        private const string DateFormat = "yyyy-MM-dd";

        private ServiceJourneyRulesResponse _serviceJourneyRulesResponse;
        private ServiceJourneyRulesConfigResult _serviceJourneyRulesConfigResult;
        private UserSessionRequest _userSessionRequest;
        private UserProfile _userProfile;
        private EmisUserSession _emisUserSession;
        private CitizenIdUserSession _citizenIdUserSession;
        private UserSession _userSession;
        private EmisConnectionToken _connectionToken;
        private string _apiSessionId;
        private string _name;
        private int _sessionTimeoutSeconds;
        private GpSessionCreateResult _sessionCreateResult;
        private const string CsrfRequestToken = "dskhfakserhhvjcgbfdsh";
        private const int MinimumAppAge = 13;
        private const string CookieDomain = "CookieDomain";
        private const int PrescriptionsDefaultLastNumberMonthsToDisplay = 12;
        private const int SessionTimeoutMinutes = 10;
        private const int MinimumLinkageAge = 16;
        private readonly DateTimeOffset? _currentTermsConditionsEffectiveDate = DateTimeOffset.Now;
        private string _serviceDeskReference;
        private SessionConfigurationSettings _sessionConfigSettings;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _userSessionRequest = _fixture.Freeze<UserSessionRequest>();
            _userProfile = _fixture.Freeze<UserProfile>();
            _userProfile.DateOfBirth = DateTime.Now.ToString(DateFormat, CultureInfo.InvariantCulture);
            _connectionToken = _fixture.Create<EmisConnectionToken>();
            _userProfile.Im1ConnectionToken = _connectionToken.SerializeJson();
            _name = _fixture.Create<string>();
            _sessionTimeoutSeconds = SessionTimeoutMinutes * 60;

            _apiSessionId = _fixture.Create<string>();
            
            _serviceDeskReference = _fixture.Create<string>();

            _emisUserSession = _fixture.Create<EmisUserSession>();
            _emisUserSession.OdsCode = _userProfile.OdsCode;
            _emisUserSession.NhsNumber = _userProfile.NhsNumber;

            _sessionCreateResult =
                new GpSessionCreateResult.Success(_name, _emisUserSession);

            _citizenIdUserSession = new CitizenIdUserSession { AccessToken = _userProfile.AccessToken };

            _mockServiceDeskErrorReferenceGenerator = _fixture.Freeze<Mock<IErrorReferenceGenerator>>();

            _mockCitizenIdSessionService = _fixture.Freeze<Mock<ICitizenIdSessionService>>();
            _mockCitizenIdSessionService
                .Setup(x => x.Create(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier,
                    _userSessionRequest.RedirectUrl))
                .Returns(Task.FromResult(new CitizenIdSessionResult
                {
                    StatusCode = (int) HttpStatusCode.OK,
                    DateOfBirth =
                        DateTime.ParseExact(_userProfile.DateOfBirth, DateFormat, CultureInfo.InvariantCulture),
                    Im1ConnectionToken = _userProfile.Im1ConnectionToken,
                    NhsNumber = _userProfile.NhsNumber,
                    OdsCode = _userProfile.OdsCode,
                    Session = _citizenIdUserSession
                }));

            _serviceJourneyRulesResponse = _fixture.Create<ServiceJourneyRulesResponse>();

            _sessionConfigSettings = _fixture.Freeze<SessionConfigurationSettings>();
            
            _mockServiceJourneyRulesService = _fixture.Freeze<Mock<IServiceJourneyRulesService>>();

            _serviceJourneyRulesConfigResult =
                new ServiceJourneyRulesConfigResult.Success(_serviceJourneyRulesResponse);
            _mockServiceJourneyRulesService.Setup(x => x.GetServiceJourneyRulesForOdsCode(_userProfile.OdsCode));

            _mockServiceJourneyRulesService
                .Setup(x =>
                    x.GetServiceJourneyRulesForOdsCode(_userProfile.OdsCode))
                .Returns(Task.FromResult(_serviceJourneyRulesConfigResult));

            _userSession = new UserSession
            {
                CsrfToken = CsrfRequestToken,
                GpUserSession = _emisUserSession,
                CitizenIdUserSession = _citizenIdUserSession
            };

            _mockOdsCodeLookup = _fixture.Freeze<Mock<IOdsCodeLookup>>();
            _mockOdsCodeLookup
                .Setup(x => x.LookupSupplier(_userProfile.OdsCode))
                .Returns(Task.FromResult(Option.Some(Supplier.Emis)));

            _mockSessionService = _fixture.Freeze<Mock<ISessionService>>();
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber))
                .Returns(Task.FromResult(_sessionCreateResult));

            _mockTokenValidationService = _fixture.Freeze<Mock<ITokenValidationService>>();
            _mockTokenValidationService
                .Setup(x => x.IsValidConnectionTokenFormat(_userProfile.Im1ConnectionToken))
                .Returns(true);

            _mockGpSystem = _fixture.Freeze<Mock<IGpSystem>>();
            _mockGpSystem.SetupGet(x => x.Supplier).Returns(Supplier.Emis);
            _mockGpSystem
                .Setup(x => x.GetTokenValidationService())
                .Returns(_mockTokenValidationService.Object);

            _mockGpSystem
                .Setup(x => x.GetSessionService())
                .Returns(_mockSessionService.Object);

            _mockGpSystemFactory = _fixture.Freeze<Mock<IGpSystemFactory>>();
            _mockGpSystemFactory
                .Setup(x => x.CreateGpSystem(Supplier.Emis))
                .Returns(_mockGpSystem.Object);

            _mockSessionCacheService = _fixture.Freeze<Mock<ISessionCacheService>>();
            _mockSessionCacheService
                .Setup(x => x.CreateUserSession(It.IsAny<UserSession>()))
                .Returns(Task.FromResult(_apiSessionId));

            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _authenticationServiceMock
                .Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object) null));
            var serviceProviderMock = new Mock<IServiceProvider>();
            var httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();

            _mockIm1CacheService = _fixture.Freeze<Mock<IIm1CacheService>>();
            _mockIm1CacheService.Setup(x => x.DeleteIm1ConnectionToken(_connectionToken.Im1CacheKey))
                .Returns(Task.FromResult(true)).Verifiable();

            _mockSessionMapper = _fixture.Freeze<Mock<ISessionMapper>>();
            _mockSessionMapper.Setup(x => x.Map(It.IsAny<HttpContext>(),
                    _emisUserSession, _citizenIdUserSession))
                .Returns(_userSession);

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(_authenticationServiceMock.Object);

            httpContextMock.SetupGet(h => h.Response).Returns(responseMock.Object);
            httpContextMock.SetupGet(h => h.RequestServices).Returns(serviceProviderMock.Object);
            httpContextMock.SetupGet(h => h.Items).Returns(new Dictionary<object, object>());

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

            _mockLogger = _fixture.Freeze<Mock<ILogger<SessionController>>>();
            _mockLogger.SetupLogger(LogLevel.Information, $"NhsNumber={_userProfile.NhsNumber}", null);

            _mockOdsCodeMassager = _fixture.Freeze<Mock<IOdsCodeMassager>>();
            _mockOdsCodeMassager.Setup(x => x.CheckOdsCode(_userProfile.OdsCode)).Returns(_userProfile.OdsCode);

            _configurationSettings = _fixture.Freeze<ConfigurationSettings>();
            _configurationSettings.CookieDomain = CookieDomain;
            _configurationSettings.PrescriptionsDefaultLastNumberMonthsToDisplay =
                PrescriptionsDefaultLastNumberMonthsToDisplay;
            _configurationSettings.DefaultSessionExpiryMinutes = SessionTimeoutMinutes;
            _configurationSettings.DefaultHttpTimeoutSeconds = _sessionTimeoutSeconds;
            _configurationSettings.MinimumAppAge = MinimumAppAge;
            _configurationSettings.MinimumLinkageAge = MinimumLinkageAge;
            _configurationSettings.CurrentTermsConditionsEffectiveDate = _currentTermsConditionsEffectiveDate;

            _systemUnderTest = _fixture.Create<SessionController>();

            _systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };
        }

        [TestMethod]
        public async Task Post_CIDUserProfileCallFails_ReturnsBadRequest()
        {
            // Arrange
            _mockCitizenIdSessionService
                .Setup(x =>
                    x.Create(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier,
                        _userSessionRequest.RedirectUrl))
                .Returns(Task.FromResult(new CitizenIdSessionResult()
                {
                    StatusCode = (int) HttpStatusCode.BadRequest
                }))
                .Verifiable();
            
            _mockServiceDeskErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Login,
                    StatusCodes.Status400BadRequest, SourceApi.None))
                .Returns(_serviceDeskReference)
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };
            
            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

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
                        _userSessionRequest.RedirectUrl))
                .Returns(Task.FromResult(new CitizenIdSessionResult
                {
                    StatusCode = (int) HttpStatusCode.BadGateway
                }))
                .Verifiable();
            _mockServiceDeskErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Login,
                    StatusCodes.Status502BadGateway, SourceApi.NhsLogin))
                .Returns(_serviceDeskReference)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

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
            _mockOdsCodeLookup
                .Setup(x => x.LookupSupplier(_userProfile.OdsCode))
                .Returns(Task.FromResult(Option.None<Supplier>()))
                .Verifiable();
            _mockServiceDeskErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(
                    It.IsAny<ErrorTypes.LoginOdsCodeNotFoundOrNotSupported>()))
                .Returns(_serviceDeskReference)
                .Verifiable();

            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockOdsCodeLookup.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should()
                    .Be(Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.VerifyNoOtherCalls();
            _mockLogger.Verify();
        }

        [TestMethod]
        public async Task Post_Im1ConnectionTokenIsInvalidFormat_ReturnsForbidden()
        {
            // Arrange
            _mockServiceDeskErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginForbidden>()))
                .Returns(_serviceDeskReference)
                .Verifiable();
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            _mockAuditor.Setup(x => x.AuditSessionEvent(
                    _citizenIdUserSession.AccessToken,
                    _userProfile.NhsNumber,
                    Supplier.Emis,
                    AuditingOperations.SessionCreateRequest,
                    "Attempting to create Session",
                    It.IsAny<object[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            
            _mockAuditor.Setup(x => x.AuditSessionEvent(
                    _citizenIdUserSession.AccessToken,
                    _userProfile.NhsNumber,
                    Supplier.Emis,
                    AuditingOperations.SessionCreateResponse,
                    "Failed to validate Im1 connection",
                    It.IsAny<object[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _mockTokenValidationService
                .Setup(x => x.IsValidConnectionTokenFormat(_userProfile.Im1ConnectionToken))
                .Returns(false)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockTokenValidationService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.Verify();
            _mockLogger.Verify();
        }

        [TestMethod]
        public async Task Post_Im1ConnectionTokenFailsAuthenticationWithGpSupplier_ReturnsForbidden()
        {
            // Arrange
            _mockServiceDeskErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Login, StatusCodes.Status403Forbidden,
                    SourceApi.None))
                .Returns(_serviceDeskReference)
                .Verifiable();
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };
            _mockAuditor.Setup(x => x.AuditSessionEvent(
                    _citizenIdUserSession.AccessToken,
                    _userProfile.NhsNumber,
                    Supplier.Emis,
                    AuditingOperations.SessionCreateRequest,
                    "Attempting to create Session",
                    It.IsAny<object[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            _mockAuditor.Setup(x => x.AuditSessionEvent(
                    _citizenIdUserSession.AccessToken,
                    _userProfile.NhsNumber,
                    Supplier.Emis,
                    AuditingOperations.SessionCreateResponse,
                    "Creating the session failed with status code: '403'", It.IsAny<object[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            var sessionCreateResult = new GpSessionCreateResult.Forbidden();
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber))
                .ReturnsAsync(sessionCreateResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockSessionService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.Verify();
            _mockLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GpSupplierSessionCreateFails_Returns502BadGateway()
        {
            // Arrange
            _mockServiceDeskErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(ErrorCategory.Login, StatusCodes.Status502BadGateway,
                    Supplier.Emis))
                .Returns(_serviceDeskReference)
                .Verifiable();
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            _mockAuditor.Setup(x => x.AuditSessionEvent(
                    _citizenIdUserSession.AccessToken,
                    _userProfile.NhsNumber,
                    Supplier.Emis,
                    AuditingOperations.SessionCreateRequest,
                    "Attempting to create Session",
                    It.IsAny<object[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();
            _mockAuditor.Setup(x => x.AuditSessionEvent(
                    _citizenIdUserSession.AccessToken,
                    _userProfile.NhsNumber,
                    Supplier.Emis,
                    AuditingOperations.SessionCreateResponse,
                    "Creating the session failed with status code: '502'",
                    It.IsAny<object[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var sessionCreateResult = new GpSessionCreateResult.BadGateway();
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber))
                .ReturnsAsync(sessionCreateResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockSessionService.Verify();
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockAuditor.Verify();
            _mockLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GetServiceJourneyRulesForOdsReturnsNotFound_Returns500InternalServerError()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _userSession.GpUserSession.HasLinkedAccounts = true;
            
            _mockServiceDeskErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginServiceJourneyRulesOdsCodeNotFound>()))
                .Returns(_serviceDeskReference)
                .Verifiable();
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };
            _serviceJourneyRulesConfigResult =
                new ServiceJourneyRulesConfigResult.NotFound();

            _mockServiceJourneyRulesService
                .Setup(x =>
                    x.GetServiceJourneyRulesForOdsCode(_userProfile.OdsCode))
                .Returns(Task.FromResult(_serviceJourneyRulesConfigResult))
                .Verifiable();

            _mockAuditor.Setup(x => x.AuditSessionEvent(
                    _citizenIdUserSession.AccessToken,
                    _userProfile.NhsNumber,
                    Supplier.Emis,
                    AuditingOperations.SessionCreateResponse,
                    "Retrieving Service Journey Rules failed with status code: '404'",
                    It.IsAny<object[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockServiceJourneyRulesService.Verify();
            _mockAuditor.Verify();
            _mockLogger.Verify();
        }

        [TestMethod]
        public async Task Post_GetServiceJourneyRulesForOdsReturnsInternalServerError_Returns500InternalServerError()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _userSession.GpUserSession.HasLinkedAccounts = true;
            
            _mockServiceDeskErrorReferenceGenerator
                .Setup(x => x.GenerateAndLogErrorReference(It.IsAny<ErrorTypes.LoginServiceJourneyRulesOtherError>()))
                .Returns(_serviceDeskReference)
                .Verifiable();
            var expectedValue = new PfsErrorResponse
            {
                ServiceDeskReference = _serviceDeskReference
            };

            _serviceJourneyRulesConfigResult =
                new ServiceJourneyRulesConfigResult.InternalServerError();

            _mockServiceJourneyRulesService
                .Setup(x =>
                    x.GetServiceJourneyRulesForOdsCode(_userProfile.OdsCode))
                .Returns(Task.FromResult(_serviceJourneyRulesConfigResult))
                .Verifiable();

            _mockAuditor.Setup(x => x.AuditSessionEvent(
                    _citizenIdUserSession.AccessToken,
                    _userProfile.NhsNumber,
                    Supplier.Emis,
                    AuditingOperations.SessionCreateResponse,
                    "Retrieving Service Journey Rules failed with status code: '500'",
                    It.IsAny<object[]>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            var objectResult = result.Should().BeAssignableTo<ObjectResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
                objectResult.Value.Should().BeEquivalentTo(expectedValue);
            }

            _mockServiceJourneyRulesService.Verify();
            _mockAuditor.Verify();
            _mockLogger.Verify();
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsUsersSessionResponse()
        {
            _sessionConfigSettings.ProxyEnabled = true;
            _userSession.GpUserSession.HasLinkedAccounts = true;
            
            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            var expectedUserSessionResponse = new UserSessionResponse
            {
                Name = _name,
                SessionTimeout = _sessionTimeoutSeconds,
                OdsCode = _userProfile.OdsCode,
                DateOfBirth = DateTime.ParseExact(_userProfile.DateOfBirth, DateFormat,
                    CultureInfo.InvariantCulture),
                NhsNumber = _userProfile.NhsNumber,
                AccessToken = _userProfile.AccessToken,
                ServiceJourneyRules = _serviceJourneyRulesResponse
            };

            using (new AssertionScope())
            {
                var createdResultValue = result.Should().BeAssignableTo<CreatedResult>().Subject.Value;
                var actualUserSessionResponse =
                    createdResultValue.Should().BeAssignableTo<UserSessionResponse>().Subject;
                actualUserSessionResponse.Name.Should().Be(expectedUserSessionResponse.Name);
                actualUserSessionResponse.SessionTimeout.Should().Be(expectedUserSessionResponse.SessionTimeout);
                actualUserSessionResponse.Token.Should().Be(CsrfRequestToken);
                actualUserSessionResponse.OdsCode.Should().Be(expectedUserSessionResponse.OdsCode);
                actualUserSessionResponse.DateOfBirth.Should().Be(expectedUserSessionResponse.DateOfBirth);
                actualUserSessionResponse.NhsNumber.Should().Be(expectedUserSessionResponse.NhsNumber);
                actualUserSessionResponse.AccessToken.Should().Be(expectedUserSessionResponse.AccessToken);
                actualUserSessionResponse.ServiceJourneyRules.Should()
                    .BeEquivalentTo(expectedUserSessionResponse.ServiceJourneyRules);
            }
        }

        [TestMethod]
        public async Task Post_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _userSession.GpUserSession.HasLinkedAccounts = true;
            
            _mockAuditor.Setup(x => x.AuditSessionEvent(
                    _citizenIdUserSession.AccessToken,
                    _userProfile.NhsNumber,
                    Supplier.Emis,
                    AuditingOperations.SessionCreateRequest,
                    "Attempting to create Session",
                    It.IsAny<object[]>()))
                .Returns(Task.CompletedTask);
            _mockAuditor.Setup(x => x.Audit(AuditingOperations.SessionCreateResponse,
                    "Session successfully created.", It.IsAny<object[]>()))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockCitizenIdSessionService.VerifyAll();
            _mockGpSystem.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            _mockSessionCacheService.VerifyAll();
            _mockOdsCodeLookup.VerifyAll();
            _mockSessionService.VerifyAll();
            _authenticationServiceMock.VerifyAll();
            _mockSessionMapper.VerifyAll();
            _mockAuditor.VerifyAll();
            _mockLogger.VerifyAll();
            _mockServiceJourneyRulesService.VerifyAll();
        }
        
        [TestMethod]
        public async Task Post_Im1ConnectionTokenHasCacheKey_AttemptsToDeleteFromCache()
        {   // Arrange
            _sessionConfigSettings.ProxyEnabled = true;
            _userSession.GpUserSession.HasLinkedAccounts = true;
            
            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockIm1CacheService.Verify();
        }

        [TestMethod]
        public async Task Post_Im1ConnectionTokenIsAGuid_DoesNotAttemptToDeleteFromCache()
        {
            // Arrange
            _userProfile.Im1ConnectionToken = _fixture.Create<Guid>().ToString();

            _mockCitizenIdSessionService
                .Setup(x => x.Create(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier,
                    _userSessionRequest.RedirectUrl))
                .Returns(Task.FromResult(new CitizenIdSessionResult
                {
                    StatusCode = (int) HttpStatusCode.OK,
                    DateOfBirth =
                        DateTime.ParseExact(_userProfile.DateOfBirth, DateFormat, CultureInfo.InvariantCulture),
                    Im1ConnectionToken = _userProfile.Im1ConnectionToken,
                    NhsNumber = _userProfile.NhsNumber,
                    OdsCode = _userProfile.OdsCode,
                    Session = _citizenIdUserSession
                }));

            _mockIm1CacheService.Reset();
            _mockIm1CacheService.Setup(x => x.DeleteIm1ConnectionToken(It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockIm1CacheService.Verify(x => x.DeleteIm1ConnectionToken(It.IsAny<string>()), Times.Never);
        }
    }
}