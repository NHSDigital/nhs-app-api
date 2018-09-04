using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.Areas.Session.Models;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.CitizenId.Models;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionControllerPostTests
    {
        private SessionController _systemUnderTest;
        private IFixture _fixture;
        private Mock<ICitizenIdService> _mockCitizenIdService;
        private Mock<IGpSystem> _mockGpSystem;
        private Mock<IOdsCodeLookup> _mockOdsCodeLookup;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<ISessionService> _mockSessionService;
        private Mock<ITokenValidationService> _mockTokenValidationService;
        private Mock<IGpSystemFactory> _mockGpSystemFactory;
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private Mock<IOptions<ConfigurationSettings>> _configurationSettings;
        private Mock<IAntiforgery> _mockAntiforgery;
        private Mock<IAuditor> _mockAuditor;
        private Mock<IMinimumAgeValidator> _mockMinimumAgeValidator;
        private const string DATE_FORMAT = "yyyy-MM-dd";


        private UserSessionRequest _userSessionRequest;
        private UserProfile _userProfile;
        private string _apiSessionId;
        private string _name;
        private int _sessionTimeoutMinutes;
        private int _sessionTimeoutSeconds;
        private SessionCreateResult _sessionCreateResult;
        private const string CsrfRequestToken = "dskhfakserhhvjcgbfdsh";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            _userSessionRequest = _fixture.Freeze<UserSessionRequest>();
            _userProfile = _fixture.Freeze<UserProfile>();
            _userProfile.DateOfBirth = DateTime.Now.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);
            _name = _fixture.Create<string>();
            _sessionTimeoutMinutes = _fixture.Create<int>();
            _sessionTimeoutSeconds = _sessionTimeoutMinutes * 60;

            _configurationSettings = _fixture.Freeze<Mock<IOptions<ConfigurationSettings>>>();
            _configurationSettings
                .Setup(x => x.Value)
                .Returns(new ConfigurationSettings()
                    {
                        DefaultSessionExpiryMinutes = _sessionTimeoutMinutes,
                    });

            _apiSessionId = _fixture.Create<string>();

            _sessionCreateResult =
                new SessionCreateResult.SuccessfullyCreated(_name, new EmisUserSession());

            _mockCitizenIdService = _fixture.Freeze<Mock<ICitizenIdService>>();
            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier, _userSessionRequest.RedirectUrl))
                .Returns(Task.FromResult(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.OK, 
                    UserProfile = Option.Some(_userProfile)
                }));

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
                .Returns(Task.FromResult((object)null));
            var serviceProviderMock = new Mock<IServiceProvider>();
            var httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();


            _mockAntiforgery = _fixture.Freeze<Mock<IAntiforgery>>();
            _mockAntiforgery.Setup(x => x.GetTokens(httpContextMock.Object)).Returns(new AntiforgeryTokenSet(CsrfRequestToken, "", "", ""));

            _mockMinimumAgeValidator = _fixture.Freeze<Mock<IMinimumAgeValidator>>();
            _mockMinimumAgeValidator
                .Setup(x => x.IsValid(It.IsAny<DateTime>()))
                .Returns(true);
            
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(_authenticationServiceMock.Object);

            httpContextMock.SetupGet(h => h.Response).Returns(responseMock.Object);
            httpContextMock.SetupGet(h => h.RequestServices).Returns(serviceProviderMock.Object);
            httpContextMock.SetupGet(h => h.Items).Returns(new Dictionary<object, object>());

            _mockAuditor = _fixture.Freeze<Mock<IAuditor>>();

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
            _mockCitizenIdService
                .Setup(x =>
                    x.GetUserProfile(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier, _userSessionRequest.RedirectUrl))
                .Returns(Task.FromResult(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.BadRequest, 
                    UserProfile = Option.None<UserProfile>()
                }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockCitizenIdService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            _mockAuditor.VerifyNoOtherCalls();
        }
        
        [TestMethod]
        public async Task Post_CIDUserProfileCallFails_WithBadGateway_ReturnsBadGateway()
        {
            // Arrange
            _mockCitizenIdService
                .Setup(x =>
                    x.GetUserProfile(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier, _userSessionRequest.RedirectUrl))
                .Returns(Task.FromResult(new GetUserProfileResult
                {
                    StatusCode = HttpStatusCode.BadGateway, 
                    UserProfile = Option.None<UserProfile>()
                }))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockCitizenIdService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAuditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_Returns464ODSCodeNotSupported()
        {
            // Arrange
            _mockOdsCodeLookup
                .Setup(x => x.LookupSupplier(_userProfile.OdsCode))
                .Returns(Task.FromResult(Option.None<Supplier>()))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockOdsCodeLookup.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber);
            _mockAuditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_OdsCodeIsInvalidFormat_ReturnsForbidden()
        {
            // Arrange
            _mockTokenValidationService
                .Setup(x => x.IsValidConnectionTokenFormat(_userProfile.Im1ConnectionToken))
                .Returns(false)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockTokenValidationService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            _mockAuditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_Im1ConnectionTokenFailsAuthenticationWithGpSupplier_ReturnsForbidden()
        {
            // Arrange
            var sessionCreateResult = new SessionCreateResult.InvalidIm1ConnectionToken();
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber))
                .ReturnsAsync(sessionCreateResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockSessionService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
            _mockAuditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_GpSupplierSessionCreateFails_Returns502BadGateway()
        {
            // Arrange
            var sessionCreateResult = new SessionCreateResult.SupplierSystemUnavailable();
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode, _userProfile.NhsNumber))
                .ReturnsAsync(sessionCreateResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockSessionService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
            _mockAuditor.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsUsersSessionResponse()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            var createdResultValue = result.Should().BeAssignableTo<CreatedResult>().Subject.Value;
            var actualUserSessionResponse = createdResultValue.Should().BeAssignableTo<UserSessionResponse>().Subject;

            var expectedUserSessionResponse = new UserSessionResponse
            {
                Name = _name,
                SessionTimeout = _sessionTimeoutSeconds,
                OdsCode = _userProfile.OdsCode,
                DateOfBirth = DateTime.ParseExact(_userProfile.DateOfBirth, DATE_FORMAT,
                    CultureInfo.InvariantCulture),
                NhsNumber = _userProfile.NhsNumber,
            };

            actualUserSessionResponse.Name.Should().Be(expectedUserSessionResponse.Name);
            actualUserSessionResponse.SessionTimeout.Should().Be(expectedUserSessionResponse.SessionTimeout);
            actualUserSessionResponse.Token.Should().Be(CsrfRequestToken);
            actualUserSessionResponse.OdsCode.Should().Be(expectedUserSessionResponse.OdsCode);
            actualUserSessionResponse.DateOfBirth.Should().Be(expectedUserSessionResponse.DateOfBirth);
            actualUserSessionResponse.NhsNumber.Should().Be(expectedUserSessionResponse.NhsNumber);
        }

        [TestMethod]
        public async Task Post_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Arrange
            _mockAuditor.Setup(x => x.Audit(Constants.AuditingTitles.SessionCreateResponse, It.IsAny<string>(), It.IsAny<object[]>()));

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockCitizenIdService.VerifyAll();
            _mockGpSystem.VerifyAll();
            _mockGpSystemFactory.VerifyAll();
            _mockSessionCacheService.VerifyAll();
            _mockOdsCodeLookup.VerifyAll();
            _mockSessionService.VerifyAll();
            _authenticationServiceMock.VerifyAll();
            _mockAntiforgery.VerifyAll();
        }
        
        [TestMethod]
        public async Task Post_Im1ConnectionTokenFailsMinimumAgeValidation_Returned465FailedAgeRequirement()
        {
            // Arrange
            _mockMinimumAgeValidator
                .Setup(x => x.IsValid(It.IsAny<DateTime>())).Returns(false)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockMinimumAgeValidator.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status465FailedAgeRequirement);
            _mockAuditor.VerifyNoOtherCalls();
        }
        
        [TestMethod]
        public async Task Post_Im1ConnectionTokenFailsMinimumAgeValidation_NullDateOfBirth_Returned465FailedAgeRequirement()
        {
            // Arrange
            _fixture.Freeze<UserProfile>().DateOfBirth = null;

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockMinimumAgeValidator.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status465FailedAgeRequirement);
            _mockAuditor.VerifyNoOtherCalls();
        }
        
        [TestMethod]
        public async Task Post_Im1ConnectionTokenNoNhsNumber_Returned464NoNhsNumber()
        {
            // Arrange
            _fixture.Freeze<UserProfile>().NhsNumber = null;

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber);
            _mockAuditor.VerifyNoOtherCalls();
        }
    }
}