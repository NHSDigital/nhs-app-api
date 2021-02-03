using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Config;

namespace NHSOnline.App.Logging
{
    public static class NhsAppLogging
    {
        private static Func<Type, ILogger> _createLogger = CreateLoggerNotInitialised;

        private static ILoggingConfiguration Config => IConfiguration.Configuration.Logging;

        public static ILoggerFactory Init(INativeLog nativeLog)
        {
            try
            {
                var loggerFactory = LoggerFactory.Create(
                    builder => builder
                        .SetMinimumLevel(Config.MinimumLogLevel)
                        .AddProvider(new NativeLoggerProvider(Config.MinimumNativeLogLevel, nativeLog))
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

        public static void AddProvidersFromServiceProvider(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            var services = serviceProvider.GetServices<ILoggerProvider>();
            foreach (var provider in services)
            {
                loggerFactory.AddProvider(provider);
            }
        }

        public static ILogger CreateLogger<T>() => _createLogger(typeof(T));

        public static ILogger CreateLogger(Type type) => _createLogger(type);

        private static ILogger CreateLoggerNotInitialised(Type type)
            => throw new InvalidOperationException($"Cannot create logger of type {type.FullName} as logging has not been initialised");
    }
}
