using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;
using UnitTestHelper;

namespace NHSOnline.Backend.Auth.UnitTests.CitizenId
{
    [TestClass]
    public sealed class CitizenIdServiceTests : IDisposable
    {
        private IFixture _fixture;
        private CitizenIdService _systemUnderTest;
        private Mock<ICitizenIdClient> _citizenIdClientMock;
        private Mock<ICitizenIdSigningKeysService> _citizenIdSigningKeysMock;
        private Mock<IJwtTokenService<IdToken>> _idTokenService;
        private Mock<ILogger<CitizenIdService>> _loggerMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _citizenIdClientMock = _fixture.Freeze<Mock<ICitizenIdClient>>();
            _idTokenService = _fixture.Freeze<Mock<IJwtTokenService<IdToken>>>();
            _citizenIdSigningKeysMock = _fixture.Freeze<Mock<ICitizenIdSigningKeysService>>();
            _loggerMock = _fixture.Freeze<Mock<ILogger<CitizenIdService>>>();
            _systemUnderTest = _fixture.Create<CitizenIdService>();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetUserProfile_AuthCodeNullOrWhiteSpace_ReturnsNone(string authCode)
        {
            // Arrange
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert
            actualResult.UserProfile.HasValue.Should().BeFalse();
            actualResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetUserProfile_CodeVerifierNullOrWhiteSpace_ReturnsNone(string codeVerifier)
        {
            // Arrange
            var authCode = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert
            actualResult.UserProfile.HasValue.Should().BeFalse();
            actualResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetUserProfile_RedirectUrlNullOrWhiteSpace_ReturnsNone(string redirectUrl)
        {
            // Arrange
            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert
            actualResult.UserProfile.HasValue.Should().BeFalse();
            actualResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.BadRequest)]
        [DataRow(HttpStatusCode.NotFound)]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadGateway)]
        public async Task GetUserProfile_TokenCallUnsuccessful_ReturnsNone(HttpStatusCode statusCode)
        {
            // Arrange
            var tokenResponse = new CitizenIdApiObjectResponse<Token>(statusCode)
            {
                Body = null,
                ErrorResponse = new ErrorResponse { Error = "invalid_grant" },
            };

            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier, redirectUrl))
                .ReturnsAsync(tokenResponse);

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert
            _citizenIdClientMock.VerifyAll();
            actualResult.UserProfile.HasValue.Should().BeFalse();

            var mappedStatusCode = actualResult.StatusCode;

            mappedStatusCode.Should().Be(statusCode == HttpStatusCode.BadRequest
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.BadGateway);
        }

        [TestMethod]
        public async Task GetUserProfile_HappyPath_ReturnsMappedUserProfile()
        {
            // Arrange 
            var token = _fixture.Create<Token>();
            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();
            var subject = _fixture.Create<string>();

            var tokenResponse = new CitizenIdApiObjectResponse<Token>(HttpStatusCode.OK)
            {
                Body = token,
                ErrorResponse = null
            };

            var bearerToken = tokenResponse.Body.AccessToken;

            var signingKeys = Mock.Of<JsonWebKeySet>();
            var signingKeysResponse = Option.Some(signingKeys);

            var idToken = _fixture
                .Build<IdToken>()
                .With(x => x.Subject, subject)
                .Create();
            var tokenServiceResponse = Option.Some(idToken);

            var userInfo = _fixture.Build<UserInfo>()
                .With(x => x.Subject, subject)
                .Create();

            var userProfileResponse = new CitizenIdApiObjectResponse<UserInfo>(HttpStatusCode.OK)
            {
                Body = userInfo,
                ErrorResponse = null
            };

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier, redirectUrl))
                .ReturnsAsync(tokenResponse);
            _citizenIdClientMock
                .Setup(x => x.GetUserInfo(bearerToken))
                .ReturnsAsync(userProfileResponse);

            _citizenIdSigningKeysMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);

            _idTokenService
                .Setup(x => x.ReadToken(token.IdToken, signingKeys))
                .Returns(tokenServiceResponse);

            // Act 
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert 
            _citizenIdSigningKeysMock.VerifyAll();
            _idTokenService.VerifyAll();
            _citizenIdClientMock.VerifyAll();
            actualResult.UserProfile.HasValue.Should().BeTrue();

            var actualUserProfile = actualResult.UserProfile.ValueOrFailure();
            actualUserProfile.Im1ConnectionToken.Should().Be(userProfileResponse.Body.Im1ConnectionToken);
            actualUserProfile.OdsCode.Should().Be(userProfileResponse.Body.GpIntegrationCredentials.OdsCode);
            actualUserProfile.AccessToken.Should().Be(tokenResponse.Body.AccessToken);

            actualResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [TestMethod]
        public async Task GetUserProfile_SigningKeysFails_ReturnsNone()
        {
            // Arrange 
            var token = _fixture.Create<Token>();
            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();

            var tokenResponse = new CitizenIdApiObjectResponse<Token>(HttpStatusCode.OK)
            {
                Body = token,
                ErrorResponse = null
            };

            var signingKeysResponse = Option.None<JsonWebKeySet>();

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier, redirectUrl))
                .ReturnsAsync(tokenResponse);

            _citizenIdSigningKeysMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);

            // Act 
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert 
            _citizenIdSigningKeysMock.VerifyAll();
            _citizenIdClientMock.VerifyAll();

            actualResult.UserProfile.HasValue.Should().BeFalse();
            actualResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            _loggerMock.VerifyLogger(LogLevel.Error,
                "Failed to get signing keys", Times.Once());
        }

        [TestMethod]
        public async Task GetUserProfile_ExchangeAuthTokenFails_ReturnsNone()
        {
            // Arrange 
            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();

            var tokenResponse = new CitizenIdApiObjectResponse<Token>(HttpStatusCode.BadRequest)
            {
                Body = null,
                ErrorResponse = new ErrorResponse { Error = "invalid_grant" }
            };

            var signingKeys = Mock.Of<JsonWebKeySet>();
            var signingKeysResponse = Option.Some(signingKeys);

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier, redirectUrl))
                .ReturnsAsync(tokenResponse);

            _citizenIdSigningKeysMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);

            // Act 
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert 
            _citizenIdSigningKeysMock.VerifyAll();
            _citizenIdClientMock.VerifyAll();

            actualResult.UserProfile.HasValue.Should().BeFalse();
            actualResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            _loggerMock.VerifyLogger(LogLevel.Error,
                "Failed to exchange auth token for access token", Times.Once());
        }

        [TestMethod]
        public async Task GetUserProfile_IdTokenReadFails_ReturnsNone()
        {
            // Arrange 
            var token = _fixture.Create<Token>();
            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();

            var tokenResponse = new CitizenIdApiObjectResponse<Token>(HttpStatusCode.OK)
            {
                Body = token,
                ErrorResponse = null
            };

            var signingKeys = Mock.Of<JsonWebKeySet>();
            var signingKeysResponse = Option.Some(signingKeys);

            var tokenServiceResponse = Option.None<IdToken>();

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier, redirectUrl))
                .ReturnsAsync(tokenResponse);

            _citizenIdSigningKeysMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);

            _idTokenService
                .Setup(x => x.ReadToken(token.IdToken, signingKeys))
                .Returns(tokenServiceResponse);

            // Act 
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert 
            _citizenIdSigningKeysMock.VerifyAll();
            _idTokenService.VerifyAll();
            _citizenIdClientMock.VerifyAll();
            _citizenIdClientMock.VerifyNoOtherCalls();

            actualResult.UserProfile.HasValue.Should().BeFalse();
            actualResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            _loggerMock.VerifyLogger(LogLevel.Error,
                "Failed to read ID Token", Times.Once());
        }

        [TestMethod]
        public async Task GetUserProfile_SubjectMismatch_ReturnsNone()
        {
            // Arrange 
            var token = _fixture.Create<Token>();
            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();

            var tokenResponse = new CitizenIdApiObjectResponse<Token>(HttpStatusCode.OK)
            {
                Body = token,
                ErrorResponse = null
            };

            var bearerToken = tokenResponse.Body.AccessToken;

            var signingKeys = Mock.Of<JsonWebKeySet>();
            var signingKeysResponse = Option.Some(signingKeys);

            var idToken = _fixture.Create<IdToken>();
            var tokenServiceResponse = Option.Some(idToken);

            var userInfo = _fixture.Create<UserInfo>();
            var userProfileResponse = new CitizenIdApiObjectResponse<UserInfo>(HttpStatusCode.OK)
            {
                Body = userInfo,
                ErrorResponse = null
            };

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier, redirectUrl))
                .ReturnsAsync(tokenResponse);
            _citizenIdClientMock
                .Setup(x => x.GetUserInfo(bearerToken))
                .ReturnsAsync(userProfileResponse);

            _citizenIdSigningKeysMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);

            _idTokenService
                .Setup(x => x.ReadToken(token.IdToken, signingKeys))
                .Returns(tokenServiceResponse);

            // Act 
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert 
            _citizenIdSigningKeysMock.VerifyAll();
            _idTokenService.VerifyAll();
            _citizenIdClientMock.VerifyAll();
            actualResult.UserProfile.HasValue.Should().BeFalse();
            actualResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            _loggerMock.VerifyLogger(LogLevel.Error,
                "Value of subject claim differed between Token and UserInfo responses", Times.Once());
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("     ")]
        public async Task GetUserProfile_AccessTokenNullOrWhiteSpace_ReturnsNone(string accessToken)
        {
            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(accessToken);

            // Assert
            actualResult.Should().NotBeNull();
            actualResult.UserProfile.HasValue.Should().BeFalse();
            actualResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [TestMethod]
        public async Task GetUserProfile_ValidAccessToken_ReturnsMappedUserProfile()
        {
            // Arrange
            var subject = _fixture.Create<string>();

            var accessToken = JwtToken.Generate(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim("nhs_number", "9987574309"),
            });

            var userProfileResponse = new CitizenIdApiObjectResponse<UserInfo>(HttpStatusCode.OK)
            {
                Body = _fixture.Build<UserInfo>()
                    .With(u => u.Subject, subject)
                    .Create(),
                ErrorResponse = null
            };

            _citizenIdClientMock
                .Setup(x => x.GetUserInfo(accessToken))
                .ReturnsAsync(userProfileResponse);

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(accessToken);

            // Assert
            actualResult.Should().NotBeNull();
            actualResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            actualResult.UserProfile.HasValue.Should().BeTrue();

            var actualUserProfile = actualResult.UserProfile.ValueOrFailure();
            actualUserProfile.Im1ConnectionToken.Should().Be(userProfileResponse.Body.Im1ConnectionToken);
            actualUserProfile.OdsCode.Should().Be(userProfileResponse.Body.GpIntegrationCredentials.OdsCode);
            actualUserProfile.AccessToken.Should().Be(accessToken);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}