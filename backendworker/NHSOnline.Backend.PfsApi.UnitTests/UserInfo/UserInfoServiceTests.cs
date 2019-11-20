using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.UserInfo;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.UserInfo
{
    [TestClass]
    public class UserInfoServiceTests
    {
        private UserInfoService _systemUnderTest;
        private IFixture _fixture;
        private Mock<IUserInfoClient> _userInfoClient;
        private Mock<HttpContext> _httpContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _httpContext = HttpContextGetAccessTokenHelper.CreateMockHttpContext(_fixture);

            _userInfoClient = _fixture.Freeze<Mock<IUserInfoClient>>();
            _systemUnderTest = _fixture.Create<UserInfoService>();
        }
        
        [TestMethod]
        public async Task Update_Success()
        {
            // Arrange
            _userInfoClient.Setup(x => x.Post(It.IsAny<string>(), It.IsAny<HttpContext>()))          
                .ReturnsAsync(
                    new UserInfoResponse(HttpStatusCode.Created));
            
            // Act
           var result = await _systemUnderTest.Update(_fixture.Create<string>(), _httpContext.Object);

            // Assert
            _userInfoClient.VerifyAll();
            result.Should().BeOfType<UserInfoResult.Success>();
        }

        [TestMethod]
        [DataRow(HttpStatusCode.InternalServerError)]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.Forbidden)]
        public async Task Update_ClientResponseNotSuccessful_ReturnsBadGateway(HttpStatusCode httpStatusCode)
        {
            // Arrange
            _userInfoClient.Setup(x => x.Post(It.IsAny<string>(), It.IsAny<HttpContext>()))          
                .ReturnsAsync(new UserInfoResponse(httpStatusCode));
            
            // Act
            var result = await _systemUnderTest.Update(_fixture.Create<string>(), _httpContext.Object);

            // Assert
            _userInfoClient.VerifyAll();
            result.Should().BeOfType<UserInfoResult.BadGateway>();
        }
        
        [TestMethod]
        public async Task Update_ClientThrowsHttpException_ReturnsBadGateway()
        {
            // Arrange
            _userInfoClient.Setup(x => x.Post(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws<HttpRequestException>();
                
            // Act
            var result =  await _systemUnderTest.Update(_fixture.Create<string>(), _httpContext.Object);

            // Assert
            _userInfoClient.VerifyAll();
            result.Should().BeAssignableTo<UserInfoResult.BadGateway>();
        }

        [TestMethod]
        public async Task Update_ClientThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _userInfoClient.Setup(x => x.Post(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws<Exception>();
                
            // Act
            var result =  await _systemUnderTest.Update(_fixture.Create<string>(), _httpContext.Object);

            // Assert
            _userInfoClient.VerifyAll();
            result.Should().BeAssignableTo<UserInfoResult.InternalServerError>();
        }
    }
}