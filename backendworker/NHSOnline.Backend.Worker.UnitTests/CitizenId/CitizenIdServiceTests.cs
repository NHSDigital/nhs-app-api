using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.CitizenId.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.CitizenId
{
    [TestClass]
    public class CitizenIdServiceTests
    {
        private IFixture _fixture;
        private CitizenIdService _systemUnderTest;
        private Mock<ICitizenIdClient> _citizenIdClientMock;
        private Mock<ICitizenIdSigningKeysService> _citizenIdSigningKeysMock;
        private Mock<IJwtTokenService<UserProfile>> _idTokenService;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _citizenIdClientMock = _fixture.Freeze<Mock<ICitizenIdClient>>();
            _idTokenService = _fixture.Freeze<Mock<IJwtTokenService<UserProfile>>>();
            _citizenIdSigningKeysMock = _fixture.Freeze<Mock<ICitizenIdSigningKeysService>>();
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
            var tokenResponse = new CitizenIdClient.CitizenIdApiObjectResponse<Token>(HttpStatusCode.BadRequest)
            {
                Body = null,
                ErrorResponse = new ErrorResponse { Error = "invalid_grant" },
                StatusCode = statusCode
            };

            var authCode = _fixture.Create<string>();
            var codeVerifier = _fixture.Create<string>();
            var redirectUrl = _fixture.Create<string>();

            _citizenIdClientMock
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier, redirectUrl))
                .ReturnsAsync(tokenResponse);

            // Act
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl);

            // Assert?:
            _citizenIdClientMock.VerifyAll();
            actualResult.UserProfile.HasValue.Should().BeFalse();

            var mappedStatusCode = actualResult.StatusCode;
            
            if (statusCode == HttpStatusCode.BadRequest)
            {
                mappedStatusCode.Should().Be(HttpStatusCode.BadRequest);               
            }
            else
            {           
                mappedStatusCode.Should().Be(HttpStatusCode.BadGateway);  
            }
        }
        
        [TestMethod] 
        public async Task GetUserProfile_HappyPath_ReturnsMappedUserProfile() 
        { 
            // Arrange 
            var token = _fixture.Create<Token>(); 
            var authCode = _fixture.Create<string>(); 
            var codeVerifier = _fixture.Create<string>(); 
            var redirectUrl = _fixture.Create<string>(); 
            
            var tokenResponse = new CitizenIdClient.CitizenIdApiObjectResponse<Token>(HttpStatusCode.OK) 
            { 
                Body = token, 
                ErrorResponse = null 
            };
            
            var signingKeys = Mock.Of<JsonWebKeySet>();
            var signingKeysResponse = Option.Some(signingKeys);
            
            var userProfile = _fixture.Create<UserProfile>();
            var tokenServiceReponse = Option.Some(userProfile);
 
            _citizenIdClientMock 
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier, redirectUrl)) 
                .ReturnsAsync(tokenResponse);

            _citizenIdSigningKeysMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);
 
            _idTokenService 
                .Setup(x => x.ReadToken(token.IdToken, signingKeys))
                .Returns(tokenServiceReponse); 
 
            // Act 
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl); 
 
            // Assert 
            _citizenIdSigningKeysMock.VerifyAll();
            _idTokenService.VerifyAll();
            _citizenIdClientMock.VerifyAll(); 
            actualResult.UserProfile.HasValue.Should().BeTrue(); 
 
            var actualUserProfile = actualResult.UserProfile.ValueOrFailure(); 
            actualUserProfile.Im1ConnectionToken.Should().Be(userProfile.Im1ConnectionToken); 
            actualUserProfile.OdsCode.Should().Be(userProfile.OdsCode);
           
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
            
            var tokenResponse = new CitizenIdClient.CitizenIdApiObjectResponse<Token>(HttpStatusCode.OK) 
            { 
                Body = token, 
                ErrorResponse = null 
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
        } 
        
        [TestMethod] 
        public async Task GetUserProfile_ExchangeAuthTokenFails_ReturnsNone() 
        { 
            // Arrange 
            var authCode = _fixture.Create<string>(); 
            var codeVerifier = _fixture.Create<string>(); 
            var redirectUrl = _fixture.Create<string>(); 
            
            var tokenResponse = new CitizenIdClient.CitizenIdApiObjectResponse<Token>(HttpStatusCode.BadRequest)
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
        } 
        
        [TestMethod] 
        public async Task GetUserProfile_IdTokenReadFails_ReturnsNone() 
        { 
            // Arrange 
            var token = _fixture.Create<Token>(); 
            var authCode = _fixture.Create<string>(); 
            var codeVerifier = _fixture.Create<string>(); 
            var redirectUrl = _fixture.Create<string>(); 
            
            var tokenResponse = new CitizenIdClient.CitizenIdApiObjectResponse<Token>(HttpStatusCode.OK) 
            { 
                Body = token, 
                ErrorResponse = null 
            };
            
            var signingKeys = Mock.Of<JsonWebKeySet>();
            var signingKeysResponse = Option.Some(signingKeys);
            
            var tokenServiceReponse = Option.None<UserProfile>();
 
            _citizenIdClientMock 
                .Setup(x => x.ExchangeAuthToken(authCode, codeVerifier, redirectUrl)) 
                .ReturnsAsync(tokenResponse);

            _citizenIdSigningKeysMock
                .Setup(x => x.GetSigningKeys())
                .ReturnsAsync(signingKeysResponse);
 
            _idTokenService 
                .Setup(x => x.ReadToken(token.IdToken,signingKeys))
                .Returns(tokenServiceReponse); 
 
            // Act 
            var actualResult = await _systemUnderTest.GetUserProfile(authCode, codeVerifier, redirectUrl); 
 
            // Assert 
            _citizenIdSigningKeysMock.VerifyAll();
            _idTokenService.VerifyAll();
            _citizenIdClientMock.VerifyAll(); 
            
            actualResult.UserProfile.HasValue.Should().BeFalse(); 
            actualResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }        
    }
}
