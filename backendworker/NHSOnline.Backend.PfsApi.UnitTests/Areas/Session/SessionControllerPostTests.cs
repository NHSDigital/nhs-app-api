using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    [TestClass]
    public sealed class SessionControllerTests
    {
        private SessionControllerTestContext Context { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new SessionControllerTestContext();
        }

        [TestMethod]
        public async Task Post_LogsLastFiveCharactersOfJTI()
        {
            // Arrange
            var userSessionRequest = CreateUserSessionRequest();

            Context.Mocks.SessionExpiryCookieCreator.Setup(x => x.CreateSessionExpiryToken()).Returns("ey");

            Context.Mocks.AntiForgery.Setup(x => x.GetTokens(It.IsAny<HttpContext>()))
                .Returns(new AntiforgeryTokenSet("requestToken", "", "", ""));

            var citizenUserSession = new CitizenIdUserSession();
            var userSession = new P5UserSession("csrf", citizenUserSession)
            {
                Key = "12345"
            };

            var resultMock = new CreateSessionResult.Success(userSession);

            Context.Mocks.SessionCreator.Setup(x => x.CreateSession(It.IsAny<CreateSessionRequest>()))
                .ReturnsAsync(resultMock);

            Context.Mocks.SessionResultVisitor.Setup(x => x.Visit(It.IsAny<CreateSessionResult.Success>(),
                It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new Func<IActionResult>(
                () => null));

            // Act
            await CreateSystemUnderTest().Post(userSessionRequest);

            // Assert
            Context.Mocks.Logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals("User Session Key JTI ending: 12345", o.ToString(), StringComparison.OrdinalIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        private UserSessionRequest CreateUserSessionRequest()
        {
            var request = new UserSessionRequest()
            {
                AuthCode = "4de013da-75cb-48d2-a9ae-050a3b9a1470",
                CodeVerifier = "6ksXxZm5WiyDWqFVdob_yQfF0CrTav6zfIPS2oJL4oU",
                RedirectUrl = "http://web.local.bitraft.io:3000/auth-return"
            };

            return request;
        }

        private SessionController CreateSystemUnderTest() => Context.CreateSystemUnderTest();
    }
}