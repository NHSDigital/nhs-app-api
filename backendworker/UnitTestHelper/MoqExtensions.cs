using System;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;

namespace UnitTestHelper
{
    public static class MoqExtensions
    {
        public static ISetup<ILogger<T>> SetupLogger<T>(
            this Mock<ILogger<T>> mockLogger,
            LogLevel logLevel,
            string message,
            Exception exception)
        {
            return mockLogger.Setup(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    ItIsLogMessage(message),
                    exception,
                    ItIsAnyFormatter));
        }

        public static void VerifyLogger<T>(
            this Mock<ILogger<T>> mockLogger,
            LogLevel logLevel,
            Times times)
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    ItIsAnyFormatter),
                times);
        }

        public static void VerifyLogger(
            this Mock<ILogger> mockLogger,
            LogLevel logLevel,
            string message,
            Times times)
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    ItIsLogMessage(message),
                    It.IsAny<Exception>(),
                    ItIsAnyFormatter),
                times);
        }

        public static void VerifyLogger<T>(
            this Mock<ILogger<T>> mockLogger,
            LogLevel logLevel,
            string message,
            Times times)
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    ItIsLogMessage(message),
                    It.IsAny<Exception>(),
                    ItIsAnyFormatter),
                times);
        }

        public static void VerifyLogger<TException>(
            this Mock<ILogger> mockLogger,
            LogLevel logLevel,
            Times times) where TException : Exception
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<TException>(),
                    ItIsAnyFormatter),
                times);
        }

        public static void VerifyLogger<T, TException>(
            this Mock<ILogger<T>> mockLogger,
            LogLevel logLevel,
            string message,
            Times times) where TException : Exception
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    ItIsLogMessage(message),
                    It.IsAny<TException>(),
                    ItIsAnyFormatter),
                times);
        }

        public static void VerifyLogger<T>(
            this Mock<ILogger<T>> mockLogger,
            LogLevel logLevel,
            string message,
            Exception exception,
            Times times)
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    ItIsLogMessage(message),
                    exception,
                    ItIsAnyFormatter),
                times);
        }

        private static It.IsAnyType ItIsLogMessage(string message)
            => It.Is<It.IsAnyType>((state, _) => state.ToString().Contains(message, StringComparison.InvariantCulture));

        private static Func<It.IsAnyType, Exception, string> ItIsAnyFormatter
            => It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true);
    }
}