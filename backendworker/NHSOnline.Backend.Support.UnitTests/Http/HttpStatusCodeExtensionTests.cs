using System.Net;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.Support.UnitTests.Http
{
    [TestClass]
    public class HttpStatusCodeExtensionTests
    {
        [TestMethod]
        public void HttpStatusCode_200_ReturnsTrue()
        {
            HttpStatusCode code = HttpStatusCode.OK;

            var result = code.IsSuccessStatusCode();

            result.Should().BeTrue();
        }

        [TestMethod]
        public void HttpStatusCode_299_ReturnsTrue()
        {
            HttpStatusCode code = HttpStatusCode.OK;

            var result = code.IsSuccessStatusCode();

            result.Should().BeTrue();
        }

        [TestMethod]
        public void HttpStatusCode_502_ReturnsFalse()
        {
            HttpStatusCode code = HttpStatusCode.BadGateway;

            var result = code.IsSuccessStatusCode();

            result.Should().BeFalse();
        }

        [TestMethod]
        public void HttpStatusCode_400_ReturnsFalse()
        {
            HttpStatusCode code = HttpStatusCode.BadRequest;

            var result = code.IsSuccessStatusCode();

            result.Should().BeFalse();
        }
    }
}
