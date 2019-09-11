using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.Support.UnitTests.Sanitization
{
    [TestClass]
    public class HtmlSanitizerTest
    {
        private IHtmlSanitizer _htmlSanitize;
        private HashSet<string> _whitelist;

        [TestInitialize]
        public void TestInitialize()
        {
            _whitelist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "em",
                "span"
            };
        }

        [TestMethod]
        public void SanitizeHtmlBlacklistSuccess()
        {
            // Arrange
            const string testHtml = "Test <span>1111</span><script>onLoad='test'</script>";
            const string expectedHtml = "Test <span>1111</span>";

            // Act
            _htmlSanitize = new HtmlSanitizer();
            var sanitizedHtml = _htmlSanitize.SanitizeHtml(testHtml, null);

            // Assert
            Assert.IsTrue(sanitizedHtml.Equals(expectedHtml, StringComparison.Ordinal));
        }

        [TestMethod]
        public void SanitizeHtmlWhitelistSuccess()
        {
            // Arrange
            const string testHtml = "Test <span>1111</span><script>onLoad='test'</script>";
            const string expectedHtml = "Test <span>1111</span>";

            // Act
            _htmlSanitize = new HtmlSanitizer();
            var sanitizedHtml = _htmlSanitize.SanitizeHtml(testHtml, _whitelist);

            // Assert
            Assert.IsTrue(sanitizedHtml.Equals(expectedHtml, StringComparison.Ordinal));
        }
    }
}