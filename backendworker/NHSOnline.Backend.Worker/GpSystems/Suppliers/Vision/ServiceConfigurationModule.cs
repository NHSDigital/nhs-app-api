using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILoggerFactory _loggerFactory;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {           
            var configValue = configuration.ConfigurationSettings().GetOrWarn("DefaultHttpTimeoutSeconds",
                _loggerFactory.CreateLogger<ServiceConfigurationModule>());
            var timeout = int.Parse(configValue, CultureInfo.InvariantCulture);
            
            services.AddSingleton<IGpSystem, VisionGpSystem>();
            services.AddSingleton<IVisionConfig, VisionConfig>();
            services.AddSingleton<IVisionClient, VisionClient>();
            
            services.AddTransient<VisionTokenValidationService>();

            var certificateService = services.BuildServiceProvider().GetRequiredService<ICertificateService>();          

            services.AddHttpClient<VisionHttpClient>(client => 
                {
                    client.Timeout = TimeSpan.FromSeconds(timeout);
                })
                .ConfigurePrimaryHttpMessageHandler((() =>
                        new VisionHttpClientHandler(configuration,
                            _loggerFactory.CreateLogger<VisionHttpClientHandler>(),
                            certificateService)
                    ));

            base.ConfigureServices(services, configuration);
        }
    }
}