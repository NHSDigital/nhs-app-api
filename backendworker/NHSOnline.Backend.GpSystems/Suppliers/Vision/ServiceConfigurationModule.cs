using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        private readonly ILogger<ServiceConfigurationModule> _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        protected override Supplier Supplier => Supplier.Vision;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGpSystem, VisionGpSystem>();
            services.AddSingleton<IVisionPFSConfig, VisionPFSConfig>();
            services.AddSingleton<IVisionLinkageConfig, VisionLinkageConfig>();
            services.AddSingleton<IVisionPFSClient, VisionPFSClient>();
            services.AddSingleton<IVisionLinkageClient, VisionLinkageClient>();
            services.AddSingleton<IVisionClient, VisionClient>();

            services.AddTransient<VisionTokenValidationService>();
            
            services.AddTransient<VisionPFSHttpRequestIdentifier>();
            services.AddTransient<VisionLinkageHttpRequestIdentifier>();
            services.AddSingleton<VisionHttpClientHandler>();

            services.AddHttpClient<VisionPFSHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<VisionHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<VisionPFSHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<VisionPFSHttpRequestIdentifier>>();

            services.AddHttpClient<VisionLinkageHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<VisionHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<VisionLinkageHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<VisionLinkageHttpRequestIdentifier>>();

            OdsCodeMassager.IsEnabled = bool.TrueString.Equals(
                    configuration.GetOrWarn("VISION_ODS_REMAP_ENABLED", _logger),
                    StringComparison.OrdinalIgnoreCase);
                         
            base.ConfigureServices(services, configuration);
        }
    }
}