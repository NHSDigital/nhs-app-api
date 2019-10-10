using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Support.UnitTests.Logging
{
    [TestClass]
    public sealed class HttpContextLoggerScopeTests
    {
        private const string SessionId = "fasdf-asfasf-asfsf-asgasg";
        private const string NoSessionMessage = "SessionId:{No Session yet}";
        
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
    }
}
