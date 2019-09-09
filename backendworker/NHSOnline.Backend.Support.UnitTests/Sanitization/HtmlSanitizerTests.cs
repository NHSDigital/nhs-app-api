using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.Support.UnitTests.Sanitization
{
    [TestClass]
    public class HtmlSanitizerTests
    {
        private IHtmlSanitizer _htmlSanitizer;
        private HashSet<string> _whitelist;

        [TestInitialize]
        public void TestInitialize()
        {
            _htmlSanitizer = new HtmlSanitizer();
            
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
            var sanitizedHtml = _htmlSanitizer.SanitizeHtml(testHtml, null);

            // Assert
            sanitizedHtml.Should().Be(expectedHtml);
        }

        [TestMethod]
        public void SanitizeHtmlWhitelistSuccess()
        {
            // Arrange
            const string testHtml = "Test <span>1111</span><script>onLoad='test'</script>";
            const string expectedHtml = "Test <span>1111</span>";

            // Act
            var sanitizedHtml = _htmlSanitizer.SanitizeHtml(testHtml, _whitelist);

            // Assert
            sanitizedHtml.Should().Be(expectedHtml);
        }
        
                [TestMethod]
        public async Task HtmlSanitizer_RemovesRestrictedHtml_WhenSanitizingHtmlFile()
        {
            // Arrange
            var htmlToSanitize =
                await ReadTestDataFile(
                    "NHSOnline.Backend.Support.UnitTests.Sanitization.HtmlDataToSanitize.html");
           
            // Act
            var sanitizedHtml = _htmlSanitizer.SanitizeHtml(htmlToSanitize, null);

            // Assert
            sanitizedHtml.Should().NotBeNullOrEmpty();
            sanitizedHtml.Should().NotContainAny("<script", "<meta", "javascript:", "vbscript:", "onclick", "url(");
        }
        
        [TestMethod]
        public async Task HtmlSanitizer_ReturnsSafeHtml_WhenSanitizingHtmlFile()
        {
            // Arrange
            var htmlToSanitize =
                await ReadTestDataFile(
                     "NHSOnline.Backend.Support.UnitTests.Sanitization.HtmlDataToSanitize.html");

            // Act
            var sanitizedHtml = _htmlSanitizer.SanitizeHtml(htmlToSanitize, null);

            // Assert
            sanitizedHtml.Should().NotBeNullOrEmpty();
            sanitizedHtml.Should().ContainAll("Safe Anchor", "Safe Paragraph", "Safe Table Cell");
        }

        private static async Task<string> ReadTestDataFile(string resourceFile)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream(resourceFile);

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}