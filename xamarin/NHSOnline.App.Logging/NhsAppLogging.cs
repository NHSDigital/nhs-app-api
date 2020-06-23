using System;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Logging
{
    public static class NhsAppLogging
    {
        private static ILoggerFactory _loggerFactory = null!;

        public static ILoggerFactory Init()
        {
            try
            {
                // TODO: Set Minimum Level from configuration once available
                _loggerFactory = LoggerFactory.Create(
                    builder => builder
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddDebug());

                _loggerFactory.CreateLogger(typeof(NhsAppLogging)).LogInformation("Logging initialised");

                return _loggerFactory;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to initialise logging: {0}", e);
                throw;
            }
        }

        public static ILogger<T> CreateLogger<T>() => _loggerFactory.CreateLogger<T>();
    }
}
