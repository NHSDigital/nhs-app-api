using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.Support.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static void ConfigureLogging(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
            => AddProviders(p => loggingBuilder.AddProvider(p), configuration);

        public static void ConfigureLogging(this ILoggerFactory loggerFactory, IConfiguration configuration)
            => AddProviders(loggerFactory.AddProvider, configuration);

        private static void AddProviders(Action<NhsAppLoggerProvider> addProvider, IConfiguration configuration)
        {
            var logSettings = LoggingSettings.GetSettings(configuration);

#pragma warning disable CA2000 // Dispose objects before losing scope - logging provider instance lives for the lifetime of the service
            var standardOutLoggingProvider = new NhsAppLoggerProvider(Console.Out, logSettings.StandardLevel, logSettings.ErrorLevel, logSettings.CensorFilters);
            addProvider(standardOutLoggingProvider);

            var standardErrorLoggingProvider = new NhsAppLoggerProvider(Console.Error, logSettings.ErrorLevel, LogLevel.None, logSettings.CensorFilters);
            addProvider(standardErrorLoggingProvider);
#pragma warning restore CA2000 // Dispose objects before losing scope
        }
    }
}