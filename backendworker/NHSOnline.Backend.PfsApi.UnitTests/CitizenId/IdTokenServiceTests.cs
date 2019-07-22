using System.Security.Claims;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.CitizenId;

namespace NHSOnline.Backend.PfsApi.UnitTests.CitizenId
{
    [TestClass]
    public class IdTokenServiceTests
    {
        private IFixture _fixture;
        private Mock<ITokenValidationParameterBuilder> _mockParameterBuilder;
        private Mock<ISecurityTokenValidator> _mockJwtTokenValidator;
        private Mock<ICitizenIdConfig> _mockConfig;
        private IdTokenService _systemUnderTest;
        private string _issuer;
        private string _audience;
        private string _subject;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _issuer = _fixture.Create<string>();
            _audience = _fixture.Create<string>();
            _subject = _fixture.Create<string>();

            _mockConfig = _fixture.Freeze<Mock<ICitizenIdConfig>>();

            _mockConfig
                .Setup(x => x.Issuer)
                .Returns(_issuer);
            _mockConfig
                .Setup(x => x.ClientId)
                .Returns(_audience);

            _mockParameterBuilder = _fixture.Freeze<Mock<ITokenValidationParameterBuilder>>();
            _mockJwtTokenValidator = _fixture.Freeze<Mock<ISecurityTokenValidator>>();
        }

        [TestMethod]
        public void ReadToken_HappyPath_ReturnsIdToken()
        {
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

            SecurityToken secToken;
            _mockJwtTokenValidator
                .Setup(x => x.ValidateToken(jwtToken, It.IsAny<TokenValidationParameters>(), out secToken))
                .Returns(mockPrincipal.Object)
                .Verifiable();

            var signingKeys = Mock.Of<JsonWebKeySet>();

            _systemUnderTest = _fixture.Create<IdTokenService>();
            var result = _systemUnderTest.ReadToken(jwtToken, signingKeys);

            _mockParameterBuilder.VerifyAll();
            _mockJwtTokenValidator.VerifyAll();
            mockPrincipal.VerifyAll();

            var idToken = result.ValueOrFailure();
            idToken.Subject.Should().BeEquivalentTo(_subject);
        }

        [TestMethod]
        public void ReadToken_FailWithEmptyToken_ReturnsNone()
        {
            const string jwtToken = "";

            var signingKeys = Mock.Of<JsonWebKeySet>();

            _systemUnderTest = _fixture.Create<IdTokenService>();
            var result = _systemUnderTest.ReadToken(jwtToken, signingKeys);

            result.HasValue.Should().Be(false);
        }

        [TestMethod]
        public void ReadToken_FailWhenCannotReadToken_ReturnsNone()
        {
            var jwtToken = _fixture.Create<string>();

            _mockJwtTokenValidator
                .Setup(x => x.CanReadToken(jwtToken))
                .Returns(false);

            var signingKeys = Mock.Of<JsonWebKeySet>();

            _systemUnderTest = _fixture.Create<IdTokenService>();
            var result = _systemUnderTest.ReadToken(jwtToken, signingKeys);

            _mockJwtTokenValidator.VerifyAll();

            result.HasValue.Should().Be(false);
        }

        [TestMethod]
        public void ReadToken_FailWhenTokenCannotBeValidated_ReturnsNone()
        {
            var jwtToken = _fixture.Create<string>();

            _mockJwtTokenValidator
                .Setup(x => x.CanReadToken(jwtToken))
                .Returns(true);
            _mockJwtTokenValidator
                .Setup(x => x.CanValidateToken)
                .Returns(false);

            var signingKeys = Mock.Of<JsonWebKeySet>();

            _systemUnderTest = _fixture.Create<IdTokenService>();
            var result = _systemUnderTest.ReadToken(jwtToken, signingKeys);

            _mockJwtTokenValidator.VerifyAll();

            result.HasValue.Should().Be(false);
        }

        [TestMethod]
        public void ReadToken_FailsWhenCidTokenNotFound_ReturnsNone()
        {
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

            SecurityToken secToken;
            _mockJwtTokenValidator
                .Setup(x => x.ValidateToken(jwtToken, It.IsAny<TokenValidationParameters>(), out secToken))
                .Returns(mockPrincipal.Object)
                .Verifiable();

            var signingKeys = Mock.Of<JsonWebKeySet>();

            _systemUnderTest = _fixture.Create<IdTokenService>();
            var result = _systemUnderTest.ReadToken(jwtToken, signingKeys);

            _mockParameterBuilder.VerifyAll();
            _mockJwtTokenValidator.VerifyAll();
            mockPrincipal.VerifyAll();

            result.HasValue.Should().Be(false);
        }
    }
}