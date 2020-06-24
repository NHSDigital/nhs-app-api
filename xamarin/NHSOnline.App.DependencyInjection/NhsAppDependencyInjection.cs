using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.DependencyInjection
{
    public static class NhsAppDependencyInjection
    {
        public static IServiceProvider Init(Action<IServiceCollection> configureServices, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(typeof(NhsAppDependencyInjection));
            logger.LogInformation("Initialising dependency injection");

            try
            {
                var services = ConfigureServices(configureServices, loggerFactory);

                return BuildServiceProvider(services);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to initialise dependency injection");
                throw;
            }
        }

        private static IServiceCollection ConfigureServices(Action<IServiceCollection> configureServices, ILoggerFactory loggerFactory)
        {
            var services = new ServiceCollection()
                .AddSingleton(loggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddPageFactory();

            configureServices(services);
            return services;
        }

        private static IServiceProvider BuildServiceProvider(IServiceCollection services)
        {
            var serviceProviderOptions = new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            };

            return services.BuildServiceProvider(serviceProviderOptions);
        }
    }
}
