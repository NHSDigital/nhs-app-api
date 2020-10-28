using System;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.LoggerApi.Areas.Logging.Models;
using NHSOnline.Backend.LoggerApi.Logging;
using UnitTestHelper;

namespace NHSOnline.Backend.LoggerApi.UnitTests.Logging
{
    [TestClass]
    public class LoggingServiceTests
    {
        [TestMethod]
        public void LogMessageWithNewLines_LogsEncodedMessage()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_error_message=""This%20is%20a%0A%20multiline%20message"" client_timestamp=""2020-10-26 12:32:46""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(2020, 10, 26, 12, 32, 46, 123, TimeSpan.Zero),
                Level = Level.Error,
                Message = @"This is a
 multiline message"
            });

            mockLogger.VerifyLogger(LogLevel.Error, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void LogMessageWithEqualsSign_LogsEncodedMessage()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_debug_message=""fakeKey%3DvaluePair"" client_timestamp=""2020-10-26 13:46:12""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(2020, 10, 26, 13, 46, 12, 789, TimeSpan.Zero),
                Level = Level.Debug,
                Message = @"fakeKey=valuePair"
            });

            mockLogger.VerifyLogger(LogLevel.Debug, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void LogMessageWithNonUtcTimestamp_LogsEncodedMessage()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_information_message=""CharlieDelta"" client_timestamp=""2020-10-25 00:32:30""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(2020, 10, 25, 00, 32, 30, 123, TimeSpan.FromHours(1)),
                Level = Level.Information,
                Message = @"CharlieDelta"
            });

            mockLogger.VerifyLogger(LogLevel.Information, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void LogMessageWithDoubleQuotes_LogsEncodedMessage()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_error_message=""Something%20%22quoted%22%20here"" client_timestamp=""1999-12-31 23:59:59""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(1999, 12, 31, 23, 59, 59, 999, TimeSpan.Zero),
                Level = Level.Error,
                Message = @"Something ""quoted"" here"
            });

            mockLogger.VerifyLogger(LogLevel.Error, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void LogMessageWithManyBadCharacters_LogsEncodedMessage()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_debug_message=""m*e%26s%5Es%25a@g!e%20wi%2Bt%3Dh%20b%3Fa%23d%20c%7Ch%7Ba%7Dr%3Ea%3Ccterse"" client_timestamp=""2012-03-12 20:45:46""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(2012, 03, 12, 20, 45, 46, 839, TimeSpan.FromHours(1)),
                Level = Level.Debug,
                Message = @"m*e&s^s%a@g!e wi+t=h b?a#d c|h{a}r>a<cterse"
            });

            mockLogger.VerifyLogger(LogLevel.Debug, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void LogMessageContainingUrl_LogsEncodedMessage()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_error_message=""https%3A%2F%2Fexample.org%3Fkey%3Dvalue%26key2%3D%2522quotedValue%2522"" client_timestamp=""2000-01-01 00:00:00""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(2000, 01, 01, 00, 00, 00, 0, TimeSpan.Zero),
                Level = Level.Error,
                Message = @"https://example.org?key=value&key2=%22quotedValue%22"
            });

            mockLogger.VerifyLogger(LogLevel.Error, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void LogMessageWithDebugLogLevel_LogsAtDebugLogLevel()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_debug_message=""AlphaBravo"" client_timestamp=""2032-03-14 20:10:46""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(2032, 03, 14, 20, 10, 46, 839, TimeSpan.FromHours(1)),
                Level = Level.Debug,
                Message = @"AlphaBravo"
            });

            mockLogger.VerifyLogger(LogLevel.Debug, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void LogMessageWithInformationLogLevel_LogsAtInformationLogLevel()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_information_message=""AlphaBravo"" client_timestamp=""2032-03-14 20:10:46""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(2032, 03, 14, 20, 10, 46, 839, TimeSpan.FromHours(1)),
                Level = Level.Information,
                Message = @"AlphaBravo"
            });

            mockLogger.VerifyLogger(LogLevel.Information, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void LogMessageWithErrorLogLevel_LogsAtErrorLogLevel()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_error_message=""AlphaBravo"" client_timestamp=""2032-03-14 20:10:46""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(2032, 03, 14, 20, 10, 46, 839, TimeSpan.FromHours(1)),
                Level = Level.Error,
                Message = @"AlphaBravo"
            });

            mockLogger.VerifyLogger(LogLevel.Error, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void LogMessageWithNullLogLevel_LogsAtInformationLogLevel()
        {
            var mockLogger = new Mock<ILogger<LoggingService>>();
            var loggingService = new LoggingService(mockLogger.Object);

            var expectedMessage = @"client_information_message=""AlphaBravo"" client_timestamp=""2032-03-14 20:10:46""";

            loggingService.LogMessage(new CreateLogRequest
            {
                TimeStamp = new DateTimeOffset(2032, 03, 14, 20, 10, 46, 839, TimeSpan.FromHours(1)),
                Level = null,
                Message = @"AlphaBravo"
            });

            mockLogger.VerifyLogger(LogLevel.Information, expectedMessage, Times.Once());
            mockLogger.VerifyNoOtherCalls();
        }
    }
}