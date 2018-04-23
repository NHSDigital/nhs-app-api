using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.Areas.Session.Models;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.Ods;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Session;
using NHSOnline.Backend.Worker.Session;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionControllerTests
    {
        private SessionController _systemUnderTest;
        private static IFixture _fixture;
        private static Mock<ICitizenIdService> _mockCitizenIdService;
        private static Mock<ISystemProvider> _mockSystemProvider;
        private static Mock<IOdsCodeLookup> _mockOdsCodeLookup;
        private static Mock<ISessionCacheService> _mockSessionCacheService;
        private static Mock<ISessionService> _mockSessionService;
        private static Mock<IConfiguration> _mockConfiguration;
        private static Mock<ITokenValidationService> _mockTokenValidationService;
        private static Mock<ISystemProviderFactory> _mockSystemProviderFactory;

        private static int _sessionExpiryMinutes;
        private static UserSessionRequest _userSessionRequest;
        private static UserProfile _userProfile;
        private static string _supplierSessionId;
        private static string _apiSessionId;
        private string _givenName;
        private string _familyName;
        private SessionCreateResult _sessionCreateResult;
        private CookieOptions _cookieOptions;
        private Mock<IResponseCookies> _mockResponseCookies;
        private string _cookieKey;
        private string _cookieValue;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());

            _userSessionRequest = _fixture.Freeze<UserSessionRequest>();
            _userProfile = _fixture.Freeze<UserProfile>();

            _supplierSessionId = _fixture.Create<string>();
            _givenName = _fixture.Create<string>();
            _familyName = _fixture.Create<string>();

            _apiSessionId = _fixture.Create<string>();

            _sessionCreateResult =
                new SessionCreateResult.SuccessfullyCreated(_supplierSessionId, _givenName, _familyName);

            _mockCitizenIdService = _fixture.Freeze<Mock<ICitizenIdService>>();
            _mockCitizenIdService
                .Setup(x => x.GetUserProfile(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier))
                .Returns(Task.FromResult(Option.Some(_userProfile)));

            _mockOdsCodeLookup = _fixture.Freeze<Mock<IOdsCodeLookup>>();
            _mockOdsCodeLookup
                .Setup(x => x.LookupSupplier(_userProfile.OdsCode))
                .Returns(Task.FromResult(Option.Some(SupplierEnum.Emis)));

            _mockSessionService = _fixture.Freeze<Mock<ISessionService>>();
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode))
                .Returns(Task.FromResult(_sessionCreateResult));

            _mockTokenValidationService = _fixture.Freeze<Mock<ITokenValidationService>>();
            _mockTokenValidationService
                .Setup(x => x.IsValidConnectionTokenFormat(_userProfile.Im1ConnectionToken))
                .Returns(true);

            _mockSystemProvider = _fixture.Freeze<Mock<ISystemProvider>>();
            _mockSystemProvider
                .Setup(x => x.GetTokenValidationService())
                .Returns(_mockTokenValidationService.Object);

            _mockSystemProvider
                .Setup(x => x.GetSessionService())
                .Returns(_mockSessionService.Object);

            _mockSystemProviderFactory = _fixture.Freeze<Mock<ISystemProviderFactory>>();
            _mockSystemProviderFactory
                .Setup(x => x.CreateSystemProvider(SupplierEnum.Emis))
                .Returns(_mockSystemProvider.Object);

            _mockSessionCacheService = _fixture.Freeze<Mock<ISessionCacheService>>();
            _mockSessionCacheService
                .Setup(x => x.CreateUserSession(It.IsAny<UserSession>()))
                .Returns(Task.FromResult(_apiSessionId));

            _mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
            _sessionExpiryMinutes = _fixture.Create<int>();
            _mockConfiguration.SetupGet(r => r["SESSION_EXPIRY_MINUTES"]).Returns(_sessionExpiryMinutes.ToString);

            _mockResponseCookies = _fixture.Freeze<Mock<IResponseCookies>>();
            _mockResponseCookies.Setup(x => x.Append(Cookies.SessionId, _apiSessionId, It.IsAny<CookieOptions>()))
                .Callback<string, string, CookieOptions>((key, value, options) =>
                {
                    _cookieKey = key;
                    _cookieValue = value;
                    _cookieOptions = options;
                })
                .Verifiable();

            var httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();

            httpContextMock.SetupGet(h => h.Response).Returns(responseMock.Object);
            responseMock.SetupGet(r => r.Cookies).Returns(_mockResponseCookies.Object);

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
                    x.GetUserProfile(_userSessionRequest.AuthCode, _userSessionRequest.CodeVerifier))
                .Returns(Task.FromResult(Option.None<UserProfile>()))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockCitizenIdService.Verify();
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_UnknownOdsCode_ReturnsForbidden()
        {
            // Arrange
            _mockOdsCodeLookup
                .Setup(x => x.LookupSupplier(_userProfile.OdsCode))
                .Returns(Task.FromResult(Option.None<SupplierEnum>()))
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockOdsCodeLookup.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
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
        }

        [TestMethod]
        public async Task Post_Im1ConnectionTokenFailsAuthenticationWithGpSupplier_ReturnsForbidden()
        {
            // Arrange
            var sessionCreateResult = new SessionCreateResult.InvalidIm1ConnectionToken();
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode))
                .ReturnsAsync(sessionCreateResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockSessionService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }

        [TestMethod]
        public async Task Post_GpSupplierSessionCreateFails_Returns502BadGateway()
        {
            // Arrange
            var sessionCreateResult = new SessionCreateResult.SupplierSystemUnavailable();
            _mockSessionService
                .Setup(x => x.Create(_userProfile.Im1ConnectionToken, _userProfile.OdsCode))
                .ReturnsAsync(sessionCreateResult)
                .Verifiable();

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockSessionService.Verify();
            var statusCodeResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status502BadGateway);
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsUsersNameInBodyOfResponse()
        {
            // Arrange

            // Act
            var result = await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            var createdResultValue = result.Should().BeAssignableTo<CreatedResult>().Subject.Value;
            var actualUserSessionResponse = createdResultValue.Should().BeAssignableTo<UserSessionResponse>().Subject;

            var expectedUserSessionResponse = new UserSessionResponse
            {
                FamilyName = _familyName,
                GivenName = _givenName
            };
            actualUserSessionResponse.Should().BeEquivalentTo(expectedUserSessionResponse);
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsSessionIdInCookieWithExpectedKey()
        {
            // Arrange

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockResponseCookies.Verify();
            _cookieKey.Should().Be(Cookies.SessionId);
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsSessionIdInCookieWithApiSessionIdInValue()
        {
            // Arrange

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockResponseCookies.Verify();
            _cookieValue.Should().Be(_apiSessionId);
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsSessionIdInCookieWithExpectedExpiry()
        {
            // Arrange

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockResponseCookies.Verify();
            _cookieOptions.Expires.Should().BeCloseTo(DateTimeOffset.Now.AddMinutes(_sessionExpiryMinutes));
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsSessionIdInCookieWithSecureSet()
        {
            // Arrange

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockResponseCookies.Verify();
            _cookieOptions.Secure.Should().BeTrue();
        }

        [TestMethod]
        public async Task Post_HappyPath_ReturnsSessionIdInCookieWithHttpOnlySet()
        {
            // Arrange

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockResponseCookies.Verify();
            _cookieOptions.HttpOnly.Should().BeTrue();
        }

        [TestMethod]
        public async Task Post_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Arrange

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockCitizenIdService.VerifyAll();
            _mockSystemProvider.VerifyAll();
            _mockSystemProvider.VerifyAll();
            _mockSessionCacheService.VerifyAll();
            _mockOdsCodeLookup.VerifyAll();
            _mockSessionService.VerifyAll();
            _mockConfiguration.VerifyAll();
            _mockResponseCookies.VerifyAll();
        }
    }
}