using System;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Ndop;

namespace NHSOnline.Backend.PfsApi.UnitTests.Ndop
{
    [TestClass]
    public class NdopServiceTests
    {
        private NdopService _ndopService;
        private IFixture _fixture;
        private Mock<INdopSigning> _ndopSigning;
        private Mock<Microsoft.Extensions.Configuration.IConfiguration> _configuration;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());           
           
            _configuration = _fixture.Create<Mock<Microsoft.Extensions.Configuration.IConfiguration>>(); 
            _configuration.SetupGet(x => x["NDOP_CLAIM_AUDIENCE"]).Returns("testaudience");
            _configuration.SetupGet(x => x["NDOP_CLAIM_ISSUER"]).Returns("testissuer");
            
            _ndopSigning = _fixture.Freeze<Mock<INdopSigning>>();
            _fixture.Inject(_configuration.Object);
            _ndopService = _fixture.Create<NdopService>();
        }
        
        [TestMethod]
        public void GetToken_WhenCalledAndErrorCreatingSigningCredentials_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            _ndopSigning.Setup(x => x.GetSigningCredentials()).Returns(() => null);
            const string testNhsNumber = "123456789"; 
            // Act
            
            var ndopResponse = _ndopService.GetJwtToken(testNhsNumber);
            
            // Assert
            _ndopSigning.Verify(x => x.GetSigningCredentials());
            ndopResponse.Should().BeOfType<GetNdopResult.InternalServerError>();
        }
        
         
        [TestMethod]
        public void GetToken_WhenCalledAndExceptionOccursCreatingSigningCredentials_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            _ndopSigning.Setup(x => x.GetSigningCredentials()).Throws<Exception>();
            const string testNhsNumber = "123456789";
            
            // Act
            var ndopResponse = _ndopService.GetJwtToken(testNhsNumber);
            
            // Assert
            _ndopSigning.Verify(x => x.GetSigningCredentials());
            ndopResponse.Should().BeOfType<GetNdopResult.InternalServerError>();
        }
        
        [TestMethod]
        public void GetToken_WhenCalledAndValidSigningCredentials_ReturnsSuccessfulResponseWithToken()
        {
            // Arrange
            const string key = "401b09eab3c013d4ca54922bb80";
            const string testNhsNumber = "123456789";
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));            
            var credentials = new SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256); 
            
            _ndopSigning.Setup(x => x.GetSigningCredentials()).Returns(() => credentials);

            // Act
            var ndopResponse = _ndopService.GetJwtToken(testNhsNumber);
            
            // Assert
            _ndopSigning.Verify(x => x.GetSigningCredentials());
            ndopResponse.Should().BeOfType<GetNdopResult.Success>();          
        }

        [DataTestMethod]
        [DataRow("", "testissuer")]
        [DataRow("testaudience", "")]
        public void GetJwtToken_WhenCalledWithEmptyConfigurationValues_ReturnsInternalServerErrorResponse(string claimAudience, string claimIssuer)
        {

            // Arrange
            const string key = "401b09eab3c013d4ca54922bb80";
            const string testNhsNumber = "123456789";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256);

            _ndopSigning.Setup(x => x.GetSigningCredentials()).Returns(() => credentials);
            _configuration.SetupGet(x => x["NDOP_CLAIM_AUDIENCE"]).Returns(claimAudience);
            _configuration.SetupGet(x => x["NDOP_CLAIM_ISSUER"]).Returns(claimIssuer);

            // Act
            var ndopResponse = _ndopService.GetJwtToken(testNhsNumber);

            // Assert
            _ndopSigning.Verify(x => x.GetSigningCredentials());
            ndopResponse.Should().BeOfType<GetNdopResult.InternalServerError>();
        }

        [TestMethod]
        public void GetJwtToken_WhenCalledWithEmptyCredentials_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            const string testNhsNumber = "123456789";

            _ndopSigning.Setup(x => x.GetSigningCredentials()).Returns(() => null);

            // Act
            var ndopResponse = _ndopService.GetJwtToken(testNhsNumber);

            // Assert
            _ndopSigning.Verify(x => x.GetSigningCredentials());
            ndopResponse.Should().BeOfType<GetNdopResult.InternalServerError>();

        }
    }
}