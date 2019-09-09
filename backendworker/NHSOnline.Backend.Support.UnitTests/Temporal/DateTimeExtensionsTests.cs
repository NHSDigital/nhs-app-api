using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.Support.UnitTests.Temporal
{
    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void DateTimeToYYYYMMDDString_ValidDateTime_ReturnsFormattedDateTime_Success()
        {
            // Arrange
            const string expectedDateTime = "2018-12-19";
            DateTime? testDate = DateTime.ParseExact("19/12/2018", "dd/MM/yyyy", CultureInfo.CurrentCulture);
            
            // Act
            var formattedDate = testDate.FormatToYYYYMMDD();
            
            // Assert
            formattedDate.Should().Be(expectedDateTime);
        }
        
        [TestMethod]
        public void DateTimeToYYYYMMDDString_ValidDateTime_ReturnsNull_Success()
        {
            // Arrange
            DateTime? nullDateTime = null;

            // Act
            var formattedDate = nullDateTime.FormatToYYYYMMDD();
            
            // Assert
            formattedDate.Should().BeNull();
        }
    }
}