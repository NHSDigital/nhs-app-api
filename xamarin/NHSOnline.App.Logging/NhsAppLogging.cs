using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Logging
{
    public static class NhsAppLogging
    {
        private static Func<Type, ILogger> _createLogger = CreateLoggerNotInitialised;

        public static ILoggerFactory Init()
        {
            try
            {
                // TODO: Set Minimum Level from configuration once available
                var loggerFactory = LoggerFactory.Create(
                    builder => builder
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddDebug());

                _createLogger = type => loggerFactory.CreateLogger(type);

                CreateLogger(typeof(NhsAppLogging)).LogInformation("Logging initialised");

                return loggerFactory;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to initialise logging: {0}", e);
                throw;
            }
        }

        public static ILogger CreateLogger(Type type) => _createLogger(type);

        private static ILogger CreateLoggerNotInitialised(Type type)
            => throw new InvalidOperationException($"Cannot create logger of type {type.FullName} as logging has not been initialised");
    }
}
