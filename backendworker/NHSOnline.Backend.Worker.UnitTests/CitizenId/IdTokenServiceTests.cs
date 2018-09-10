using System.Security.Claims;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.CitizenId;

namespace NHSOnline.Backend.Worker.UnitTests.CitizenId
{
    [TestClass]
    public class IdTokenServiceTests
    {
        private IFixture _fixture;
        private Mock<ITokenValidationParameterBuilder> _mockParameterBuilder;
        private Mock<ISecurityTokenValidator> _mockJwtTokenValidator;
        private Mock<ICitizenIdConfig> _mockConfig;
        private IdTokenService _systemUndertest;
        private string _issuer; 
        private string _audience;
        private string _im1Token;
        private string _odsCode;
        private string _nhsNumber;
        private string _dateOfBirth;
        private string _im1Key;
        private string _odsKey;
        private string _nhsNumberKey;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _issuer = _fixture.Create<string>();
            _audience = _fixture.Create<string>();
            _im1Token = _fixture.Create<string>();
            _odsCode = _fixture.Create<string>();
            _dateOfBirth = _fixture.Create<string>();
            _nhsNumber = _fixture.Create<string>();
            
            _im1Key = Constants.CitizenIdClaimTypes.Im1ConnectionTokenClaim;
            _odsKey = Constants.CitizenIdClaimTypes.OdscodeClaim;
            _nhsNumberKey = Constants.CitizenIdClaimTypes.NhsNumber;

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
        public void ReadToken_HappyPath_ReturnsUserInfo()
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
                .Setup(x => x.FindFirst(_im1Key))
                .Returns(new Claim(_im1Key, _im1Token));
            mockPrincipal
                .Setup(x => x.FindFirst(_odsKey))
                .Returns(new Claim(_odsKey, _odsCode));
            mockPrincipal
                .Setup(x => x.FindFirst(_nhsNumberKey))
                .Returns(new Claim(_nhsNumberKey, _nhsNumber));
            mockPrincipal
                .Setup(x => x.FindFirst(ClaimTypes.DateOfBirth))
                .Returns(new Claim(ClaimTypes.DateOfBirth, _dateOfBirth));

            SecurityToken secToken;
            _mockJwtTokenValidator
                .Setup(x => x.ValidateToken(jwtToken, It.IsAny<TokenValidationParameters>(), out secToken))
                .Returns(mockPrincipal.Object)
                .Verifiable();

            var signingKeys = Mock.Of<JsonWebKeySet>();
            
            _systemUndertest = _fixture.Create<IdTokenService>();
            var result = _systemUndertest.ReadToken(jwtToken, signingKeys);
            
            _mockParameterBuilder.VerifyAll();
            _mockJwtTokenValidator.VerifyAll();
            mockPrincipal.VerifyAll();
            
            var actualUserProfile = result.ValueOrFailure(); 
            actualUserProfile.Im1ConnectionToken.Should().Be(_im1Token);
            actualUserProfile.OdsCode.Should().Be(_odsCode);
            actualUserProfile.NhsNumber.Should().Be(_nhsNumber);
            actualUserProfile.DateOfBirth.Should().Be(_dateOfBirth);
        }
        
        [TestMethod]
        public void ReadToken_FailWithEmptyToken_ReturnsNone()
        {
            var jwtToken = "";

            var signingKeys = Mock.Of<JsonWebKeySet>();
            
            _systemUndertest = _fixture.Create<IdTokenService>();
            var result = _systemUndertest.ReadToken(jwtToken, signingKeys);

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

            _systemUndertest = _fixture.Create<IdTokenService>();
            var result = _systemUndertest.ReadToken(jwtToken, signingKeys);

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

            _systemUndertest = _fixture.Create<IdTokenService>();
            var result = _systemUndertest.ReadToken(jwtToken, signingKeys);

            _mockJwtTokenValidator.VerifyAll();

            result.HasValue.Should().Be(false);
        }
        
        [TestMethod]
        public void ReadToken_FailsWhenIm1TokenNotFound_ReturnsNone()
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
                .Setup(x => x.FindFirst(_im1Key))
                .Returns((Claim) null);
            mockPrincipal
                .Setup(x => x.FindFirst(_odsKey))
                .Returns(new Claim(_odsKey, _odsCode));

            SecurityToken secToken;
            _mockJwtTokenValidator
                .Setup(x => x.ValidateToken(jwtToken, It.IsAny<TokenValidationParameters>(), out secToken))
                .Returns(mockPrincipal.Object)
                .Verifiable();

            var signingKeys = Mock.Of<JsonWebKeySet>();
            
            _systemUndertest = _fixture.Create<IdTokenService>();
            var result = _systemUndertest.ReadToken(jwtToken, signingKeys);
            
            _mockParameterBuilder.VerifyAll();
            _mockJwtTokenValidator.VerifyAll();
            mockPrincipal.VerifyAll();
            
            result.HasValue.Should().Be(false);
        }
        
        [TestMethod]
        public void ReadToken_FailsWhenOdsCodeNotFound_ReturnsNone()
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
                .Setup(x => x.FindFirst(_im1Key))
                .Returns(new Claim(_im1Key, _im1Token));
            mockPrincipal
                .Setup(x => x.FindFirst(_odsKey))
                .Returns((Claim) null);

            SecurityToken secToken;
            _mockJwtTokenValidator
                .Setup(x => x.ValidateToken(jwtToken, It.IsAny<TokenValidationParameters>(), out secToken))
                .Returns(mockPrincipal.Object)
                .Verifiable();

            var signingKeys = Mock.Of<JsonWebKeySet>();
            
            _systemUndertest = _fixture.Create<IdTokenService>();
            var result = _systemUndertest.ReadToken(jwtToken, signingKeys);
            
            _mockParameterBuilder.VerifyAll();
            _mockJwtTokenValidator.VerifyAll();
            mockPrincipal.VerifyAll();
            
            result.HasValue.Should().Be(false);
        }
    }
}