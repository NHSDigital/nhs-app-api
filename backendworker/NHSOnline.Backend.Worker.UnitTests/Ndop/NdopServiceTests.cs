using System;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Core.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Ndop;

namespace NHSOnline.Backend.Worker.UnitTests.Ndop
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
        public async Task GetToken_WhenCalledAndErrorCreatingSigningCredentials_ReturnsUnSuccessfulResponse()
        {
            // Arrange
            _ndopSigning.Setup(x => x.GetSigningCredentials()).Returns(() => null);
            const string testNhsNumber = "123456789"; 
            // Act
            
            var ndopResponse = await _ndopService.GetJwtToken(testNhsNumber);
            
            // Assert
            _ndopSigning.Verify(x => x.GetSigningCredentials());
            ndopResponse.Should().BeOfType<GetNdopResult.Unsuccessful>();
        }
        
         
        [TestMethod]
        public async Task GetToken_WhenCalledAndExceptionOccursCreatingSigningCredentials_ReturnsUnSuccessfulResponse()
        {
            // Arrange
            _ndopSigning.Setup(x => x.GetSigningCredentials()).Throws<Exception>();
            const string testNhsNumber = "123456789";
            
            // Act
            var ndopResponse = await _ndopService.GetJwtToken(testNhsNumber);
            
            // Assert
            _ndopSigning.Verify(x => x.GetSigningCredentials());
            ndopResponse.Should().BeOfType<GetNdopResult.Unsuccessful>();
        }
        
        [TestMethod]
        public async Task GetToken_WhenCalledAndValidSigningCredentials_ReturnsSuccessfulResponseWithToken()
        {
            // Arrange
            const string key = "401b09eab3c013d4ca54922bb80";
            const string testNhsNumber = "123456789";
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));            
            var credentials = new SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256); 
            
            _ndopSigning.Setup(x => x.GetSigningCredentials()).Returns(() => credentials);

            // Act
            var ndopResponse = await _ndopService.GetJwtToken(testNhsNumber);
            
            // Assert
            _ndopSigning.Verify(x => x.GetSigningCredentials());
            ndopResponse.Should().BeOfType<GetNdopResult.SuccessfullyRetrieved>();          
        }          
    }
}