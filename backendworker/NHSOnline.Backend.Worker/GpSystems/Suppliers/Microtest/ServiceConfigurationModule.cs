using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Certificate;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ServiceConfigurationModule> _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            if (bool.TryParse(configuration.GetOrWarn("GP_PROVIDER_ENABLED_MICROTEST", _logger), out bool enabled) &&
                enabled)
            {
                var defaultHttpTimeoutSeconds = configuration.ConfigurationSettings()
                    .GetOrWarn("DefaultHttpTimeoutSeconds", _logger);

                services.AddSingleton<MicrotestHttpClientHandler>();
                var certificateService = services.BuildServiceProvider().GetRequiredService<ICertificateService>();

                services.AddHttpClient<MicrotestHttpClient>(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(int.Parse(defaultHttpTimeoutSeconds, CultureInfo.InvariantCulture));
                })
                .ConfigurePrimaryHttpMessageHandler<MicrotestHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<MicrotestHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<MicrotestHttpRequestIdentifier>>();

                services.AddSingleton<IGpSystem, MicrotestGpSystem>();
                services.AddSingleton<IMicrotestClient, MicrotestClient>();
                services.AddSingleton<IMicrotestConfig, MicrotestConfig>();
                services.AddTransient<MicrotestTokenValidationService>();
                
                _logger.LogDebug("Microtest GP Service was successfully configured");
            }
            else
            {
                _logger.LogDebug("Microtest GP Service was not configured");
            }

            base.ConfigureServices(services, configuration);
        }
    }
}