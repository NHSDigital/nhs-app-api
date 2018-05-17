using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.CitizenId.Models;

namespace NHSOnline.Backend.Worker.UnitTests.CitizenId
{
    [TestClass]
    public class CitizenIdServiceTests
    {
        private IFixture _fixture;
        private CitizenIdService _systemUnderTest;
        private Mock<ICitizenIdClient> _citizenIdClientMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _citizenIdClientMock = _fixture.Freeze<Mock<ICitizenIdClient>>();
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

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier);

            // Assert
            actualResult.HasValue.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public async Task GetUserProfile_CodeVerifierNullOrWhiteSpace_ReturnsNone(string codeVerifier)
        {
            // Arrange
            var authCode = _fixture.Create<string>();

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier);

            // Assert
            actualResult.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task GetUserProfile_TokenCallUnsuccessful_ReturnsNone()
        {
            // Arrange
            var tokenResponse = new CitizenIdClient.CitizenIdApiObjectResponse<Token>(HttpStatusCode.BadRequest)
            {
                Body = null,
                ErrorResponse = new ErrorResponse { Error = "invalid_grant", ErrorDescription = "Code not valid" }
            };

            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier))
                .ReturnsAsync(tokenResponse);

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier);

            // Assert
            _citizenIdClientMock.VerifyAll();
            actualResult.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task GetUserProfile_UserInfoCallUnsuccessful_ReturnsNone()
        {
            // Arrange
            var token = _fixture.Create<Token>();
            var tokenResponse = new CitizenIdClient.CitizenIdApiObjectResponse<Token>(HttpStatusCode.Created)
            {
                Body = token,
                ErrorResponse = null
            };

            var userProfileResponse =
                new CitizenIdClient.CitizenIdApiObjectResponse<UserInfo>(HttpStatusCode.Unauthorized)
                {
                    Body = null,
                    ErrorResponse = new ErrorResponse
                    {
                        Error = "invalid_token",
                        ErrorDescription = "Token invalid: Token is not active"
                    }
                };

            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier))
                .ReturnsAsync(tokenResponse);

            _citizenIdClientMock
                .Setup(x => x.GetUserInfo(token.AccessToken))
                .ReturnsAsync(userProfileResponse);

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier);

            // Assert
            _citizenIdClientMock.VerifyAll();
            actualResult.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task GetUserProfile_UserInfoCallSuccessful_ReturnsMappedUserProfile()
        {
            // Arrange
            var token = _fixture.Create<Token>();
            var tokenResponse = new CitizenIdClient.CitizenIdApiObjectResponse<Token>(HttpStatusCode.OK)
            {
                Body = token,
                ErrorResponse = null
            };

            var userInfo = _fixture.Create<UserInfo>();
            var userInfoResponse = new CitizenIdClient.CitizenIdApiObjectResponse<UserInfo>(HttpStatusCode.OK)
            {
                Body = userInfo,
                ErrorResponse = null
            };

            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier))
                .ReturnsAsync(tokenResponse);

            _citizenIdClientMock
                .Setup(x => x.GetUserInfo(token.AccessToken))
                .ReturnsAsync(userInfoResponse);

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier);

            // Assert
            _citizenIdClientMock.VerifyAll();
            actualResult.HasValue.Should().BeTrue();

            var actualUserProfile = actualResult.ValueOrFailure();
            actualUserProfile.Im1ConnectionToken.Should().Be(userInfo.Im1ConnectionToken);
            actualUserProfile.OdsCode.Should().Be(userInfo.OdsCode);
        }
    }
}