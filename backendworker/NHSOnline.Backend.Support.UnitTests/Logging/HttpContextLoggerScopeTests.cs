using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.Support.UnitTests.Logging
{
    [TestClass]
    public sealed class HttpContextLoggerScopeTests
    {
        private const string SessionId = "fasdf-asfasf-asfsf-asgasg";
        private const string NoSessionMessage = "SessionId:{No Session yet}";
        private const string MainNhsNumber = "123 456 7890";
        private const string ProxyNhsNumber = "098 765 4321";
 
        [TestMethod]
        public void ToString_UserSessionNotPresent_ReturnsNoSessionMessage()
        {
            var httpContext = CreateHttpContext(Option.None<P5UserSession>());
            var systemUnderTest = new HttpContextLoggerScope(httpContext);
            var result = systemUnderTest.ToString();

            result.Should().Be(NoSessionMessage);
        }

        [TestMethod]
        public void ToString_UserSessionPresent_ReturnsSessionIDMessage()
        {
            var httpContext = CreateHttpContext(
                new P9UserSession(
                    string.Empty,
                    new CitizenIdUserSession(),
                    new EmisUserSession(),
                    string.Empty)
                {
                    Key = SessionId
                });

            var systemUnderTest = new HttpContextLoggerScope(httpContext);
            var result = systemUnderTest.ToString();

            result.Should().Be($"SessionId:{SessionId}");
        }

        [TestMethod]
        public void ToString_HttpContextNull_ReturnsNoSessionMessage()
        {
            var systemUnderTest = new HttpContextLoggerScope(null);
            var result = systemUnderTest.ToString();

            result.Should().Be(NoSessionMessage);
        }

        [TestMethod]
        public void ToString_UserSessionServiceNotRegistered_ReturnsNoSessionMessage()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().BuildServiceProvider()
            };

            var systemUnderTest = new HttpContextLoggerScope(httpContext);
            var result = systemUnderTest.ToString();

            result.Should().Be(NoSessionMessage);
        }

        [TestMethod]
        public void ToString_UserSessionPresent_ProxyModeTrue_ReturnsSessionIDMessageAndProxyMessage()
        {
            var httpContext = CreateHttpContext(
                new P9UserSession(
                    string.Empty,
                    new CitizenIdUserSession(),
                    new EmisUserSession { NhsNumber = MainNhsNumber },
                    string.Empty)
                {
                    Key = SessionId
                });

            httpContext.Items.Add("LinkedAccountAuditInfo", new LinkedAccountAuditInfo
            {
                IsProxyMode = true,
                ProxyNhsNumber = ProxyNhsNumber
            });

            var systemUnderTest = new HttpContextLoggerScope(httpContext);
            var result = systemUnderTest.ToString();

            result.Should().Be($"SessionId:{SessionId} | In proxy mode");
        }

        [TestMethod]
        public void ToString_UserSessionPresent_NoLinkedAccountAuditInfo_ReturnsSessionIDMessage()
        {
            var httpContext = CreateHttpContext(
                new P9UserSession(
                    string.Empty,
                    new CitizenIdUserSession(),
                    new EmisUserSession { NhsNumber = MainNhsNumber },
                    string.Empty)
                {
                    Key = SessionId
                });

            httpContext.Items.Add("LinkedAccountAuditInfo", null);

            var systemUnderTest = new HttpContextLoggerScope(httpContext);
            var result = systemUnderTest.ToString();

            result.Should().Be($"SessionId:{SessionId}");
        }
        
        [TestMethod]
        public void ToString_UserSessionPresent_ProxyModeFalse_ReturnsSessionIDMessage()
        {
            var httpContext = CreateHttpContext(
                new P9UserSession(
                    string.Empty,
                    new CitizenIdUserSession(),
                    new EmisUserSession { NhsNumber = MainNhsNumber },
                    string.Empty)
                {
                    Key = SessionId
                });
            
            httpContext.Items.Add("LinkedAccountAuditInfo", new LinkedAccountAuditInfo
            {
                IsProxyMode = false,
                ProxyNhsNumber = ProxyNhsNumber
            });

            var systemUnderTest = new HttpContextLoggerScope(httpContext);
            var result = systemUnderTest.ToString();

            result.Should().Be($"SessionId:{SessionId}");
        }

        private static DefaultHttpContext CreateHttpContext(P5UserSession userSession)
            => CreateHttpContext(Option.Some(userSession));

        private static DefaultHttpContext CreateHttpContext(Option<P5UserSession> userSession)
        {
            var mockUserSessionService = new Mock<IUserSessionService>();
            mockUserSessionService.Setup(x => x.GetUserSession<P5UserSession>()).Returns(userSession);
            return new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().AddSingleton(mockUserSessionService.Object).BuildServiceProvider()
            };
        }
    }
}
