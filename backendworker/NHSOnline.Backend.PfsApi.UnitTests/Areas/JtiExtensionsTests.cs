using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.Areas;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas
{
    [TestClass]
    public class JtiExtensionsTests
    {
        [TestMethod]
        public void ToLoggableJti_HandlesEmptyJti_ReturnsEmptyString()
        {
            // Arrange
            var emptyJti = string.Empty;

            // Act
            var loggableJti = emptyJti.ToLoggableJti();

            // Assert
            loggableJti.Should().BeEmpty();
        }

        [TestMethod]
        public void ToLoggableJti_HandlesNullJti_ReturnsEmptyString()
        {
            // Arrange
            string emptyJti = null;

            // Act
            var loggableJti = emptyJti.ToLoggableJti();

            // Assert
            loggableJti.Should().BeEmpty();
        }

        [TestMethod]
        public void ToLoggableJti_HandlesFourCharJti_ReturnsFullJti()
        {
            // Arrange
            var jti = "char";

            // Act
            var loggableJti = jti.ToLoggableJti();

            // Assert
            loggableJti.Should().BeEquivalentTo(jti);
        }

        [TestMethod]
        public void ToLoggableJti_HandlesFiveCharJti_ReturnsFullJti()
        {
            // Arrange
            var jti = "5char";

            // Act
            var loggableJti = jti.ToLoggableJti();

            // Assert
            loggableJti.Should().BeEquivalentTo(jti);
        }

        [TestMethod]
        public void ToLoggableJti_HandlesFullCharJti_ReturnsLastFiveChars()
        {
            // Arrange
            var jti = "5943e22b-6dfd-4790-9f78-3defef1e063a";

            // Act
            var loggableJti = jti.ToLoggableJti();

            // Assert
            loggableJti.Should().BeEquivalentTo("e063a");
        }
    }
}