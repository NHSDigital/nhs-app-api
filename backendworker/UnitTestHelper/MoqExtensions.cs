using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;

namespace UnitTestHelper
{
    public static class MoqExtensions
    {
        public static Moq.Language.Flow.ISetup<ILogger<T>> SetupLogger<T>(this Mock<ILogger<T>> mockLogger,
            LogLevel logLevel, string message, Exception exception)
        {
            return mockLogger.Setup(x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(flv => flv.ToString().Contains(message, StringComparison.InvariantCulture)),
                exception,
                It.IsAny<Func<object, Exception, string>>()));
        }

        public static void VerifyLogger<T>(this Mock<ILogger<T>> mockLogger, LogLevel logLevel, Times times)
        {
            mockLogger.Verify(x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.IsAny<FormattedLogValues>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()), times);
        }

        public static void VerifyLogger(this Mock<ILogger> mockLogger, LogLevel logLevel, Type exceptionType, Times times)
        {
            mockLogger.Verify(x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.IsAny<FormattedLogValues>(),
                It.Is<Exception>(e => exceptionType.IsInstanceOfType(e)),
                It.IsAny<Func<object, Exception, string>>()), times);
        }

        public static void VerifyLogger<T>(this Mock<ILogger<T>> mockLogger, LogLevel logLevel, Type exceptionType, Times times)
        {
            mockLogger.Verify(x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.IsAny<FormattedLogValues>(),
                It.Is<Exception>(e => exceptionType.IsInstanceOfType(e)),
                It.IsAny<Func<object, Exception, string>>()), times);
        }
        
        public static void VerifyLogger<T>(this Mock<ILogger<T>> mockLogger, LogLevel logLevel, string message, Times times)
        {
            mockLogger.Verify(x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(flv => flv.ToString().Contains(message, StringComparison.InvariantCulture)),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()), times);
        }

        public static void VerifyLogger<T>(this Mock<ILogger<T>> mockLogger, LogLevel logLevel, string message,
            Type exceptionType, Times times)
        {
            mockLogger.Verify(x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(flv => flv.ToString().Contains(message, StringComparison.InvariantCulture)),
                It.Is<Exception>(e => exceptionType.IsInstanceOfType(e)),
                It.IsAny<Func<object, Exception, string>>()), times);
        }

        public static void VerifyLogger<T>(this Mock<ILogger<T>> mockLogger, LogLevel logLevel, string message,
            Exception exception, Times times)
        {
            mockLogger.Verify(x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<FormattedLogValues>(flv => flv.ToString().Contains(message, StringComparison.InvariantCulture)),
                exception,
                It.IsAny<Func<object, Exception, string>>()), times);
        }
    }
}