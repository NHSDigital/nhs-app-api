using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.Authorization;
using NHSOnline.Backend.PfsApi.Areas.Authorization.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Authorization
{
    [TestClass]
    public sealed class AuthorizationControllerTests : IDisposable
    {
        private AuthorizationController _systemUnderTest;
        private Mock<ICitizenIdService> _mockCitizenIdService;
        private Mock<ISessionCacheService> _mockSessionCacheService;
        private Mock<ILogger<AuthorizationController>> _mockLogger;
        private P5UserSession _userSession;
        private string RefreshToken = "Refresh Token";
        private string AccessToken = "Access Token";
        
        [TestInitialize]
        public void TestInitialize()
        {
            _mockCitizenIdService = new Mock<ICitizenIdService>();
            _mockSessionCacheService = new Mock<ISessionCacheService>();
            _mockLogger = new Mock<ILogger<AuthorizationController>>();

            _userSession = new P5UserSession("csrfToken", new CitizenIdUserSession { RefreshToken = RefreshToken });

            _systemUnderTest = new AuthorizationController(
                _mockCitizenIdService.Object,
                _mockSessionCacheService.Object,
                _mockLogger.Object
                );
        }

        [TestMethod]
        public async Task PostRefreshToken_ReturnsSuccessWithAccessToken_WhenServiceReturnSuccessfully()
        {
            //Arrange
            var expectedResponse = new OkObjectResult(new TokenRefreshResponse
            {
                Token = AccessToken
            });

            _mockCitizenIdService
                .Setup(x => x.RefreshAccessToken(RefreshToken))
                .ReturnsAsync(new RefreshAccessTokenResult.Success(AccessToken));

            //Act
            var result = await _systemUnderTest.RefreshAccessToken(_userSession);

            //Assert
            _mockCitizenIdService.VerifyAll();
            result.Should().BeOfType<OkObjectResult>();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [DataTestMethod]
        [DataRow(typeof(RefreshAccessTokenResult.BadGateway), StatusCodes.Status502BadGateway)]
        [DataRow(typeof(RefreshAccessTokenResult.InternalServerError), StatusCodes.Status500InternalServerError)]
        public async Task PostRefreshToken_ServiceReturnsErrorResult_ReturnsAppropriateStatusCodeResultObject(
            Type serviceResultType,
            int expectedStatusCode)
        {
            //Arrange
            _mockCitizenIdService
                .Setup(x => x.RefreshAccessToken(RefreshToken))
                .ReturnsAsync((RefreshAccessTokenResult) Activator.CreateInstance(serviceResultType));

            //Act
            var result = await _systemUnderTest.RefreshAccessToken(_userSession);

            //Assert
            _mockCitizenIdService.VerifyAll();
            var objectResult = result.Should().BeAssignableTo<StatusCodeResult>().Subject;
            using (new AssertionScope())
            {
                objectResult.StatusCode.Should().Be(expectedStatusCode);
            }
        }

        public void Dispose()
        {
            _systemUnderTest.Dispose();
        }
    }
}