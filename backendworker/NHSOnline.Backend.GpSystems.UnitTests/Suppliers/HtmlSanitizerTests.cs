using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers
{
    [TestClass]
    public class HtmlSanitizerTests
    {
        private HtmlSanitizer _htmlSanitizer;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _htmlSanitizer = new HtmlSanitizer();
        }

        [TestMethod]
        public async Task HtmlSanitizer_RemovesRestrictedHtml_WhenSanitizingHtmlFile()
        {
            // Arrange
            var htmlToSanitize =
                await ReadTestDataFile(
                    "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.TestData.HtmlDataToSanitize.html");
           
            // Act
            var sanitizedHtml = _htmlSanitizer.SanitizeHtml(htmlToSanitize);

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(sanitizedHtml));
            AssertNotContainsAny(sanitizedHtml, "<script", "<meta", "javascript:", "vbscript:", "onclick", "url(");

        }
        
        [TestMethod]
        public async Task HtmlSanitizer_ReturnsSafeHtml_WhenSanitizingHtmlFile()
        {
            // Arrange
            var htmlToSanitize =
                await ReadTestDataFile(
                     "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.TestData.HtmlDataToSanitize.html");

            // Act
            var sanitizedHtml = _htmlSanitizer.SanitizeHtml(htmlToSanitize);

            // Assert
            Assert.IsTrue(!string.IsNullOrEmpty(htmlToSanitize));
            AssertContainsAll(sanitizedHtml, "Safe Anchor", "Safe Paragraph", "Safe Table Cell");
        }

        private static void AssertNotContainsAny(string sanitizedHtml, params string[] shouldNotContain)
        {
            foreach (var htmlItem in shouldNotContain)
            {
                Assert.IsTrue(!sanitizedHtml.Contains(htmlItem, StringComparison.OrdinalIgnoreCase),
                    $"sanitized html contains {htmlItem}");
            }
        }
        
        private static void AssertContainsAll(string sanitizedHtml, params string[] shouldContain)
        {
            foreach (var htmlItem in shouldContain)
            {
                Assert.IsTrue(sanitizedHtml.Contains(htmlItem, StringComparison.OrdinalIgnoreCase),
                    $"sanitized html removed {htmlItem}");
            }
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
