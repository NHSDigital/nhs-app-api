using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestHelper
{
    public static class MockLoggersExtensions
    {
        public static IServiceCollection AddMockLoggers(this IServiceCollection service)
            => service.AddSingleton(typeof(ILogger<>), typeof(MockLoggerWrapper<>));

        public static Mock<ILogger<TCategoryName>> MockLogger<TCategoryName>(this IServiceProvider serviceProvider)
        {
            var mockLoggerWrapper =
                serviceProvider.GetRequiredService<ILogger<TCategoryName>>() as MockLoggerWrapper<TCategoryName>
                ?? throw new AssertFailedException(MockLoggerWrapperNotFoundMessage());

            return mockLoggerWrapper.Mock;

            string MockLoggerWrapperNotFoundMessage()
                => $"ILogger<{typeof(TCategoryName).Name}> is not a MockLoggerWrapper. Ensure AddMockLoggers is called";
        }

        private sealed class MockLoggerWrapper<TCategoryName> : ILogger<TCategoryName>
        {
            internal Mock<ILogger<TCategoryName>> Mock { get; } = new Mock<ILogger<TCategoryName>>();

            private ILogger<TCategoryName> LoggerImplementation => Mock.Object;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                => LoggerImplementation.Log(logLevel, eventId, state, exception, formatter);

            public bool IsEnabled(LogLevel logLevel)
                => LoggerImplementation.IsEnabled(logLevel);

            public IDisposable BeginScope<TState>(TState state)
                => LoggerImplementation.BeginScope(state);
        }
    }
}