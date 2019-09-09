using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Brothermailer.Models;
using NHSOnline.Backend.PfsApi.Brothermailer;

namespace NHSOnline.Backend.PfsApi.UnitTests.Brothermailer
{
    [TestClass]
    public class BrothermailerServiceTests
    {
        private ILogger<BrothermailerService> _logger;
        private IFixture _fixture;
        private Mock<IBrothermailerClient> _brothermailerClient;
        private IBrothermailerService _brothermailerService;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<BrothermailerService>>();
            _brothermailerClient = new Mock<IBrothermailerClient>();

            _brothermailerService = new BrothermailerService(_logger, _brothermailerClient.Object);
        }
        
        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task
            SendEmailAddress_WhenCalledWithMissingEmailAddress_ReturnsBadRequest(string emailAddress)
        {
            // Arrange
            var missingEmailRequest = 
                new BrothermailerRequest
                {
                    EmailAddress = emailAddress
                };
            
            // Act
            var result = await _brothermailerService.SendEmailAddress(missingEmailRequest);
            
            // Assert
            result.Should().BeAssignableTo<BrothermailerResult.BadRequest>();
        }
        
        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        public async Task
            SendEmailAddress_WhenCalledWithMissingOdsCode_ReturnsBadRequest(string odsCode)
        {
            // Arrange
            var missingOdsCodeRequest = 
                new BrothermailerRequest { OdsCode = odsCode };
            
            // Act
            var result = await _brothermailerService.SendEmailAddress(missingOdsCodeRequest);
            
            // Assert
            result.Should().BeAssignableTo<BrothermailerResult.BadRequest>();
        }
        
        [TestMethod]
        public async Task
            SendEmailAddress_WhenCalledWithValidEmail_ReturnsSuccessfulResponse()
        {
            // Arrange
            const string validOdsCode = "N12345";
            const string validEmailAddress = "test@test.com";
            
            var validBrothermailerRequest = 
                new BrothermailerRequest
                {
                    OdsCode = validOdsCode,
                    EmailAddress = validEmailAddress
                };

            var validBrothermailerResponse =
                new BrothermailerClient.BrothermailerApiObjectResponse(HttpStatusCode.Redirect)
                {
                    IsSuccess = true,
                    IsInvalidEmail = false
                };
            
            _brothermailerClient.Setup(x => x.SendEmailAddress(validBrothermailerRequest))
                .Returns(Task.FromResult(validBrothermailerResponse));
            
            // Act
            var result = await _brothermailerService.SendEmailAddress(validBrothermailerRequest);
            
            // Assert
            result.Should().BeAssignableTo<BrothermailerResult.Success>();
        }
        
        [TestMethod]
        public async Task
            SendEmailAddress_WhenCalledWithInvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            const string validOdsCode = "N12345";
            const string invalidEmail = "test.com";
            
            var invalidEmailRequest = 
                new BrothermailerRequest
                {
                    OdsCode = validOdsCode,
                    EmailAddress = invalidEmail
                };

            var invalidEmailBrothermailerResponse =
                new BrothermailerClient.BrothermailerApiObjectResponse(HttpStatusCode.Redirect)
                {
                    IsSuccess = false, 
                    IsInvalidEmail = true
                };

            _brothermailerClient.Setup(x => x.SendEmailAddress(invalidEmailRequest))
                .Returns(Task.FromResult(invalidEmailBrothermailerResponse));
            
            // Act
            var result = await _brothermailerService.SendEmailAddress(invalidEmailRequest);
            
            // Assert
            result.Should().BeAssignableTo<BrothermailerResult.BadRequest>();
        }

        [TestMethod] public async Task
            SendEmailAddress_WhenCalledWithValidEmailButBrothermailerRespondsWithUnsuccessfulStatus_ReturnsInternalServerError()
        {
            // Arrange
            const string validOdsCode = "N12345";
            const string validEmail = "test@test.com";
            
            var validEmailRequest = 
                new BrothermailerRequest
                {
                    OdsCode = validOdsCode,
                    EmailAddress = validEmail
                };

            var notFoundBrothermailerResponse =
                new BrothermailerClient.BrothermailerApiObjectResponse(HttpStatusCode.NotFound);

            _brothermailerClient.Setup(x => x.SendEmailAddress(validEmailRequest))
                .Returns(Task.FromResult(notFoundBrothermailerResponse));
            
            // Act
            var result = await _brothermailerService.SendEmailAddress(validEmailRequest);
            
            // Assert
            result.Should().BeAssignableTo<BrothermailerResult.InternalServerError>();
        }
    }
}