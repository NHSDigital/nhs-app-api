using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.UnitTests.CitizenId
{
    [TestClass]
    public class IdTokenServiceTests
    {
        private IFixture _fixture;
        private Mock<ITokenValidationParameterBuilder> _mockParameterBuilder;
        private Mock<IJwtTokenValidator> _mockJwtTokenValidator;
        private Mock<ICitizenIdConfig> _mockConfig;
        private Mock<ICitizenIdSigningKeysProvider> _citizenIdClientProvider;
        private IdTokenService _systemUnderTest;
        private string _issuer;
        private string _audience;
        private string _subject;
        private string _jti;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _issuer = _fixture.Create<string>();
            _audience = _fixture.Create<string>();
            _subject = _fixture.Create<string>();
            _jti = _fixture.Create<string>();

            _mockConfig = _fixture.Freeze<Mock<ICitizenIdConfig>>();

            _mockConfig
                .Setup(x => x.Issuer)
                .Returns(_issuer);
            _mockConfig
                .Setup(x => x.ClientId)
                .Returns(_audience);

            _mockParameterBuilder = _fixture.Freeze<Mock<ITokenValidationParameterBuilder>>();
            _mockJwtTokenValidator = _fixture.Freeze<Mock<IJwtTokenValidator>>();
            _citizenIdClientProvider = _fixture.Freeze<Mock<ICitizenIdSigningKeysProvider>>();

            _citizenIdClientProvider
                .Setup(x => x.GetSigningKeys("some-key"))
                .Returns(Task.FromResult(Option.Some(new JsonWebKeySet())));
        }

        [TestMethod]
        public async Task ReadToken_HappyPath_ReturnsIdToken()
        {
            // Arrange
            var jwtToken = _fixture.Create<string>();

            _mockJwtTokenValidator
                .Setup(x => x.CanValidateToken)
                .Returns(true);

            _mockJwtTokenValidator
                .Setup(x => x.CanReadToken(jwtToken))
                .Returns(true);

            _mockParameterBuilder
                .Setup(x => x.Build(It.IsAny<JsonWebKeySet>()))
                .Returns(It.IsAny<TokenValidationParameters>())
                .Verifiable();

            var mockPrincipal = _fixture.Freeze<Mock<ClaimsPrincipal>>();

            mockPrincipal
                .Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
                .Returns(new Claim(ClaimTypes.NameIdentifier, _subject));

            mockPrincipal
                .Setup(x => x.FindFirst(JwtRegisteredClaimNames.Jti))
                .Returns(new Claim(JwtRegisteredClaimNames.Jti, _jti));

            var key = new SigningCredentials(
                new JsonWebKey()
                {
                    Kid = "some-key"
                },
                "RSA512");

            _mockJwtTokenValidator
                .Setup(x => x.ReadToken(It.IsAny<string>()))
                .Returns(
                    new JwtSecurityToken(
                        new JwtHeader(key, new Dictionary<string, string>(), "?"),
                        new JwtPayload()));

            SecurityToken secToken;
            _mockJwtTokenValidator
                .Setup(x => x.ValidateToken(jwtToken, It.IsAny<TokenValidationParameters>(), out secToken))
                .Returns(mockPrincipal.Object)
                .Verifiable();

            _systemUnderTest = _fixture.Create<IdTokenService>();

            // Act
            var result = await _systemUnderTest.ReadToken(jwtToken);

            // Assert
            _citizenIdClientProvider.VerifyAll();
            _mockParameterBuilder.VerifyAll();
            _mockJwtTokenValidator.VerifyAll();
            mockPrincipal.VerifyAll();

            var idToken = result.ValueOrFailure();
            idToken.Subject.Should().Be(_subject);
            idToken.Jti.Should().Be(_jti);
        }

        [TestMethod]
        public async Task ReadToken_FailWithEmptyToken_ReturnsNone()
        {
            // Arrange
            const string jwtToken = "";

            _systemUnderTest = _fixture.Create<IdTokenService>();

            // Act
            var result = await _systemUnderTest.ReadToken(jwtToken);

            // Assert
            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task ReadToken_FailWhenCannotReadToken_ReturnsNone()
        {
            // Arrange
            var jwtToken = _fixture.Create<string>();

            _mockJwtTokenValidator
                .Setup(x => x.CanReadToken(jwtToken))
                .Returns(false);

            _systemUnderTest = _fixture.Create<IdTokenService>();

            // Act
            var result = await _systemUnderTest.ReadToken(jwtToken);

            // Assert
            _mockJwtTokenValidator.VerifyAll();

            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task ReadToken_FailWhenTokenCannotBeValidated_ReturnsNone()
        {
            // Arrange
            var jwtToken = _fixture.Create<string>();

            _mockJwtTokenValidator
                .Setup(x => x.CanReadToken(jwtToken))
                .Returns(true);
            _mockJwtTokenValidator
                .Setup(x => x.CanValidateToken)
                .Returns(false);

            _systemUnderTest = _fixture.Create<IdTokenService>();

            // Act
            var result = await _systemUnderTest.ReadToken(jwtToken);

            // Assert
            _mockJwtTokenValidator.VerifyAll();

            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task ReadToken_FailsWhenCidTokenHasNoSubject_ReturnsNone()
        {
            // Arrange
            var jwtToken = _fixture.Create<string>();

            _mockJwtTokenValidator
                .Setup(x => x.CanValidateToken)
                .Returns(true);

            _mockJwtTokenValidator
                .Setup(x => x.CanReadToken(jwtToken))
                .Returns(true);

            _mockParameterBuilder
                .Setup(x => x.Build(It.IsAny<JsonWebKeySet>()))
                .Returns(It.IsAny<TokenValidationParameters>())
                .Verifiable();

            var mockPrincipal = _fixture.Freeze<Mock<ClaimsPrincipal>>();

            mockPrincipal
                .Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
                .Returns((Claim) null);

            var key = new SigningCredentials(
                new JsonWebKey()
                {
                    Kid = "some-key"
                },
                "RSA512");

            _mockJwtTokenValidator
                .Setup(x => x.ReadToken(It.IsAny<string>()))
                .Returns(
                    new JwtSecurityToken(
                        new JwtHeader(key, new Dictionary<string, string>(), "?"),
                        new JwtPayload()));

            SecurityToken secToken;
            _mockJwtTokenValidator
                .Setup(x => x.ValidateToken(jwtToken, It.IsAny<TokenValidationParameters>(), out secToken))
                .Returns(mockPrincipal.Object)
                .Verifiable();

            _systemUnderTest = _fixture.Create<IdTokenService>();

            // Act
            var result = await _systemUnderTest.ReadToken(jwtToken);

            // Assert
            _mockParameterBuilder.VerifyAll();
            _mockJwtTokenValidator.VerifyAll();
            mockPrincipal.VerifyAll();

            result.HasValue.Should().BeFalse();
        }

        [TestMethod]
        public async Task ReadToken_FailsWhenCidTokenHasNoJti_ReturnsNone()
        {
            // Arrange
            var jwtToken = _fixture.Create<string>();

            _mockJwtTokenValidator
                .Setup(x => x.CanValidateToken)
                .Returns(true);

            _mockJwtTokenValidator
                .Setup(x => x.CanReadToken(jwtToken))
                .Returns(true);

            _mockParameterBuilder
                .Setup(x => x.Build(It.IsAny<JsonWebKeySet>()))
                .Returns(It.IsAny<TokenValidationParameters>())
                .Verifiable();

            var mockPrincipal = _fixture.Freeze<Mock<ClaimsPrincipal>>();

            mockPrincipal
                .Setup(x => x.FindFirst(ClaimTypes.NameIdentifier))
                .Returns(new Claim(ClaimTypes.NameIdentifier, _subject));

            mockPrincipal
                .Setup(x => x.FindFirst(JwtRegisteredClaimNames.Jti))
                .Returns((Claim)null);

            var key = new SigningCredentials(
                new JsonWebKey()
                {
                    Kid = "some-key"
                },
                "RSA512");

            _mockJwtTokenValidator
                .Setup(x => x.ReadToken(It.IsAny<string>()))
                .Returns(
                    new JwtSecurityToken(
                        new JwtHeader(key, new Dictionary<string, string>(), "?"),
                        new JwtPayload()));

            SecurityToken secToken;
            _mockJwtTokenValidator
                .Setup(x => x.ValidateToken(jwtToken, It.IsAny<TokenValidationParameters>(), out secToken))
                .Returns(mockPrincipal.Object)
                .Verifiable();

            _systemUnderTest = _fixture.Create<IdTokenService>();

            // Act
            var result = await _systemUnderTest.ReadToken(jwtToken);

            // Assert
            _mockParameterBuilder.VerifyAll();
            _mockJwtTokenValidator.VerifyAll();
            mockPrincipal.VerifyAll();

            result.HasValue.Should().BeFalse();
        }
    }
}