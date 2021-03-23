using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public sealed class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILogger _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEventHubLoggerConfiguration>(GetEventHubLoggerConfiguration(configuration));
            services.AddSingleton<IEventHubLogger, EventHubLogger>();

#pragma warning disable CA2000 // Dispose objects before losing scope - logging provider instance lives for the lifetime of the service
            var pidEventHubClientConfiguration = GetPidEventHubClientConfiguration(configuration);
            services.AddSingleton<IEventHubClient>(new EventHubClient(pidEventHubClientConfiguration));

            var nonPidEventHubClientConfiguration = GetNonPidEventHubClientConfiguration(configuration);
            services.AddSingleton<IEventHubClient>(new EventHubClient(nonPidEventHubClientConfiguration));
#pragma warning restore CA2000 // Dispose objects before losing scope
        }

        private EventHubLoggerConfiguration GetEventHubLoggerConfiguration(IConfiguration configuration)
        {
            var environmentName = configuration.GetOrThrow("ENVIRONMENT_NAME", _logger);

            return new EventHubLoggerConfiguration(environmentName);
        }

        private EventHubClientConfiguration GetPidEventHubClientConfiguration(IConfiguration configuration)
        {
            var connectionString = configuration.GetOrThrow("COMMS_HUB_EVENT_HUB_PID_CONNECTION_STRING", _logger);

            return new EventHubClientConfiguration(EventHubs.CommsHubPid, connectionString);
        }

        private EventHubClientConfiguration GetNonPidEventHubClientConfiguration(IConfiguration configuration)
        {
            var connectionString = configuration.GetOrThrow("COMMS_HUB_EVENT_HUB_CONNECTION_STRING", _logger);

            return new EventHubClientConfiguration(EventHubs.CommsHubNonPid, connectionString);
        }
    }
}