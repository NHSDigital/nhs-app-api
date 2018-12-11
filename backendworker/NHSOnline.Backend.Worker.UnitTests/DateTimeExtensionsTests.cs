using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.Worker.UnitTests
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void DateTimeToYYYYMMDDString_ValidDateTime_ReturnsFormattedDateTime_Success()
        {
            // Arrange
            var expectedDateTime = "2018-12-19";
            DateTime? testDate = DateTime.ParseExact("19/12/2018", "dd/MM/yyyy", CultureInfo.CurrentCulture);
            
            // Act
            var formatedDate = testDate.FormatToYYYYMMDD();
            
            // Assert
            Assert.IsTrue(expectedDateTime.Equals(formatedDate, StringComparison.Ordinal));
        }
        
        [TestMethod]
        public void DateTimeToYYYYMMDDString_ValidDateTime_ReturnsNull_Success()
        {
            // Arrange
            DateTime? nullDateTime = null;

            // Act
            var formatedDate = nullDateTime.FormatToYYYYMMDD();
            
            // Assert
            Assert.IsNull(formatedDate);
        }
    }
}