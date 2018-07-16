using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class Module : Support.DependencyInjection.Module
    {
        private readonly ILoggerFactory _loggerFactory;

        public Module(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {           
            var configValue = configuration.ConfigurationSettings().GetOrWarn("DefaultHttpTimeoutSeconds",
                _loggerFactory.CreateLogger<Module>());
            var timeout = int.Parse(configValue);
            
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