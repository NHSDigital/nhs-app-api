using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;

namespace NHSOnline.Backend.LoggerApi.UnitTests.Areas.Logging.Models
{
    [TestClass]
    public class CreateLogRequestTests
    {
        [TestMethod]
        public void FormattedLogMessage_ReturnsCorrectText()
        {
            // Arrange
            var systemUnderTest = new CreateLogRequest
            {
                Message = "test log message",
                TimeStamp = new DateTimeOffset(2000, 6, 1, 13, 30, 1, TimeSpan.Zero),
            };

            // Act
            var message = systemUnderTest.FormattedErrorLogMessage;

            // Assert
            message.Should().Be(@"client_error_message=""test log message"" client_timestamp=""2000-06-01 13:30:01""");
        }
    }
}
