using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterPfsGpSystemsServices(this IServiceCollection services, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            services.RegisterBaseGpSystemsServices();
            
            if (enableGpSupplierConfiguration.EnableEmis)
            {
                services.RegisterEmisPfsServices();
            }

            if (enableGpSupplierConfiguration.EnableTpp)
            {
                services.RegisterTppPfsServices();
            }

            if (enableGpSupplierConfiguration.EnableVision)
            {
                services.RegisterVisionPfsServices();
            }
            
            if (enableGpSupplierConfiguration.EnableMicrotest)
            {
                services.RegisterMicrotestPfsServices();
            }

            return services;
        }
        
        public static IServiceCollection RegisterCidGpSystemsServices(this IServiceCollection services, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            services.RegisterBaseGpSystemsServices();
            
            if (enableGpSupplierConfiguration.EnableEmis)
            {
                services.RegisterEmisCidServices();
            }

            if (enableGpSupplierConfiguration.EnableTpp)
            {
                services.RegisterTppCidServices();
            }

            if (enableGpSupplierConfiguration.EnableVision)
            {
                services.RegisterVisionCidServices();
            }
            
            if (enableGpSupplierConfiguration.EnableMicrotest)
            {
                services.RegisterMicrotestCidServices();
            }

            return services;
        }

        private static IServiceCollection RegisterBaseGpSystemsServices(this IServiceCollection services)
        {
            services.AddTransient<IHtmlSanitizer, HtmlSanitizer>();
            services.AddSingleton<IGpSystemFactory, GpSystemFactory>();
            services.AddSingleton<IIm1CacheService, Im1CacheService>();
            services.AddSingleton<IIm1CacheKeyGenerator, Im1CacheKeyGenerator>();
            services.AddSingleton<IAppointmentCancellationReasonScraper, AppointmentCancellationReasonScraper>();

            return services;
        }
    }
}
