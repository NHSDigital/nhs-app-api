using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.Session;
using NHSOnline.Backend.Worker.Areas.Session.Models;
using NHSOnline.Backend.Worker.Bridges.Emis;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Session;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.Areas.Session
{
    [TestClass]
    public class SessionControllerPostTests
    {
        private SessionController _systemUnderTest;
        private IFixture _fixture;
        private Mock<ICitizenIdService> _mockCitizenIdService;
        private Mock<IBridge> _mockBridge;
        private Mock<IOdsCodeLookup> _mockOdsCodeLookup;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<ISessionService> _mockSessionService;
        private Mock<ITokenValidationService> _mockTokenValidationService;
        private Mock<IBridgeFactory> _mockBridgeFactory;
        private Mock<IAuthenticationService> _authenticationServiceMock;

        private UserSessionRequest _userSessionRequest;
        private UserProfile _userProfile;
        private string _apiSessionId;
        private string _givenName;
        private string _familyName;
        private int _sessionTimeout;
        private SessionCreateResult _sessionCreateResult;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .Customize(new ApiControllerAutoFixtureCustomization());
            
            _userSessionRequest = _fixture.Freeze<UserSessionRequest>();
            _userProfile = _fixture.Freeze<UserProfile>();

            _givenName = _fixture.Create<string>();
            _familyName = _fixture.Create<string>();
            _sessionTimeout = _fixture.Create<int>();

            _apiSessionId = _fixture.Create<string>();

            _sessionCreateResult =
                new SessionCreateResult.SuccessfullyCreated(_givenName, _familyName, new EmisUserSession(), _sessionTimeout);

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

            _mockBridge = _fixture.Freeze<Mock<IBridge>>();
            _mockBridge
                .Setup(x => x.GetTokenValidationService())
                .Returns(_mockTokenValidationService.Object);

            _mockBridge
                .Setup(x => x.GetSessionService())
                .Returns(_mockSessionService.Object);

            _mockBridgeFactory = _fixture.Freeze<Mock<IBridgeFactory>>();
            _mockBridgeFactory
                .Setup(x => x.CreateBridge(SupplierEnum.Emis))
                .Returns(_mockBridge.Object);

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

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(_authenticationServiceMock.Object);

            httpContextMock.SetupGet(h => h.Response).Returns(responseMock.Object);
            httpContextMock.SetupGet(h => h.RequestServices).Returns(serviceProviderMock.Object);

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
                FamilyName = _familyName,
                GivenName = _givenName,
                SessionTimeout = _sessionTimeout
            };

            actualUserSessionResponse.FamilyName.Should().Be(expectedUserSessionResponse.FamilyName);
            actualUserSessionResponse.GivenName.Should().Be(expectedUserSessionResponse.GivenName);
            actualUserSessionResponse.SessionTimeout.Should().Be(expectedUserSessionResponse.SessionTimeout);
        }

        [TestMethod]
        public async Task Post_HappyPath_VerifyAllExpectationsOnMocks()
        {
            // Arrange

            // Act
            await _systemUnderTest.Post(_userSessionRequest);

            // Assert
            _mockCitizenIdService.VerifyAll();
            _mockBridge.VerifyAll();
            _mockBridgeFactory.VerifyAll();
            _mockSessionCacheService.VerifyAll();
            _mockOdsCodeLookup.VerifyAll();
            _mockSessionService.VerifyAll();
            _authenticationServiceMock.VerifyAll();
        }
    }
}