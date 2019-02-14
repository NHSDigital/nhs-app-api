using System;
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
    public class TokenValidationParameterBuilderTests
    {
        private const string _signingKey =
            "{\"keys\": [{\"kty\": \"RSA\", \"e\": \"AQAB\", \"n\": \"vYKSjXOcKZI5eNvKT0BuMUAy-N7-f1-88H-Lgz5UOlyAT3wmKNHwwuz11qmovmZaKSTHk94bLIigwGIoc-nsQOahLxS1T-g0R5xN5PRvZUfK6B5W7ONX5EaXDXimKnxQLvIFXJpqzYyStkhYROTuELv70aKQNfYBrb2yZxdPNbjMzSL881awt6wiTIk76kDpzGJ0TcBBrhNKOxPU_L00FT-ASf2mKENTx2QLW8Srgw2SYo3xWhhccz1cEgjllnsX21EYNM95_hcQOBFeDfU7lYEfYGj4bX2mHE4m5up0uLAf5hOIXnfvpmtOKmUizyA9_3yPye1zJpIfZKNgtUo6-Q\"}]}";
        private IFixture _fixture;
        private Mock<ICitizenIdConfig> _config;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _config = _fixture.Freeze<Mock<ICitizenIdConfig>>();
        }

        [TestMethod]
        public void Build_HappyPath_ReturnsTokenValidationParameters()
        {
            var issuer = _fixture.Create<string>();
            var clientId = _fixture.Create<string>();
            
            _config
                .Setup(x => x.Issuer)
                .Returns(issuer);
            _config
                .Setup(x => x.ClientId)
                .Returns(clientId);
            
            var signingKeys = new JsonWebKeySet(_signingKey);
            var validSigningKeys = signingKeys.GetSigningKeys();
            
            var systemUnderTest = _fixture.Create<TokenValidationParameterBuilder>();

            var result = systemUnderTest.Build(signingKeys);

            result.ValidIssuer.Should().Be(issuer);
            result.ValidAudience.Should().Be(clientId);
            result.IssuerSigningKeys.Should().BeEquivalentTo(validSigningKeys);
            result.ValidateAudience.Should().BeTrue();
            result.ValidateIssuer.Should().BeTrue();

        }
        
        [DataRow("")]
        [DataRow(null)]
        [TestMethod]
        public void Build_EmptyIssuerString_ReturnsTokenValidationParameters(string issuer)
        {
            var clientId = _fixture.Create<string>();
            
            _config
                .Setup(x => x.Issuer)
                .Returns(issuer);
            _config
                .Setup(x => x.ClientId)
                .Returns(clientId);
            
            var signingKeys = new JsonWebKeySet(_signingKey);
            var validSigningKeys = signingKeys.GetSigningKeys();
            
            var systemUnderTest = _fixture.Create<TokenValidationParameterBuilder>();

            var result = systemUnderTest.Build(signingKeys);

            result.ValidIssuer.Should().Be(issuer);
            result.ValidAudience.Should().Be(clientId);
            result.IssuerSigningKeys.Should().BeEquivalentTo(validSigningKeys);
            result.ValidateAudience.Should().BeTrue();
            result.ValidateIssuer.Should().BeTrue();

        }
        
        [DataRow("")]
        [DataRow(null)]
        [TestMethod]
        public void Build_EmptyClientIdString_ReturnsTokenValidationParameters(string clientId)
        {
            var issuer = _fixture.Create<string>();
            
            _config
                .Setup(x => x.Issuer)
                .Returns(issuer);
            _config
                .Setup(x => x.ClientId)
                .Returns(clientId);
            
            var signingKeys = new JsonWebKeySet(_signingKey);
            var validSigningKeys = signingKeys.GetSigningKeys();
            
            var systemUnderTest = _fixture.Create<TokenValidationParameterBuilder>();

            var result = systemUnderTest.Build(signingKeys);

            result.ValidIssuer.Should().Be(issuer);
            result.ValidAudience.Should().Be(clientId);
            result.IssuerSigningKeys.Should().BeEquivalentTo(validSigningKeys);
            result.ValidateAudience.Should().BeTrue();
            result.ValidateIssuer.Should().BeTrue();

        }
        
        [TestMethod]
        public void Build_NullSigningKeys_ReturnsTokenValidationParameters()
        {
            var issuer = _fixture.Create<string>();
            var clientId = _fixture.Create<string>();
            
            _config
                .Setup(x => x.Issuer)
                .Returns(issuer);
            _config
                .Setup(x => x.ClientId)
                .Returns(clientId);
            
            var systemUnderTest = _fixture.Create<TokenValidationParameterBuilder>();
            
            Action act = () => systemUnderTest.Build(null);

            act.Should().Throw<NullReferenceException>();

        }
    }
}