using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Support.Logging;

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
            var systemUnderTest = new HttpContextLoggerScope(new DefaultHttpContext());
            var result = systemUnderTest.ToString();

            result.Should().Be(NoSessionMessage);
        }

        [TestMethod]
        public void ToString_UserSessionPresent_ReturnsSessionIDMessage()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add("UserSession", new UserSession { Key = SessionId });

            var systemUnderTest = new HttpContextLoggerScope(httpContext);
            var result = systemUnderTest.ToString();

            result.Should().Be($"SessionId:{SessionId}");
        }

        [TestMethod]
        public void ToString_UserSessionNull_ReturnsNoSessionMessage()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add("UserSession", null);

            var systemUnderTest = new HttpContextLoggerScope(httpContext);
            var result = systemUnderTest.ToString();

            result.Should().Be(NoSessionMessage);
        }

        [TestMethod]
        public void ToString_HttpContextNull_ReturnsNoSessionMessage()
        {
            var systemUnderTest = new HttpContextLoggerScope(null);
            var result = systemUnderTest.ToString();

            result.Should().Be(NoSessionMessage);
        }

        [TestMethod]
        public void ToString_HttpContextItemsNull_ReturnsNoSessionMessage()
        {
            var systemUnderTest = new HttpContextLoggerScope(new DefaultHttpContext {Items = null});
            var result = systemUnderTest.ToString();

            result.Should().Be(NoSessionMessage);
        }
        
        [TestMethod]
        public void ToString_UserSessionPresent_ProxyModeTrue_ReturnsSessionIDMessageAndProxyMessage()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add("UserSession", new UserSession
            {
                Key = SessionId,
                GpUserSession = new EmisUserSession
                {
                    NhsNumber = MainNhsNumber
                }
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
            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add("UserSession", new UserSession
            {
                Key = SessionId,
                GpUserSession = new EmisUserSession
                {
                    NhsNumber = MainNhsNumber
                }
            });
            
            httpContext.Items.Add("LinkedAccountAuditInfo", null);

            var systemUnderTest = new HttpContextLoggerScope(httpContext);
            var result = systemUnderTest.ToString();

            result.Should().Be($"SessionId:{SessionId}");
        }
        
        [TestMethod]
        public void ToString_UserSessionPresent_ProxyModeFalse_ReturnsSessionIDMessage()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items.Add("UserSession", new UserSession
            {
                Key = SessionId,
                GpUserSession = new EmisUserSession
                {
                    NhsNumber = MainNhsNumber
                }
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
    }
}
