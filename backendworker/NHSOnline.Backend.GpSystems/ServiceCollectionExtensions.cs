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
        public static IServiceCollection RegisterGpSystemsServices(this IServiceCollection services, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            services.RegisterBaseGpSystemsServices();
            services.RegisterGpSystems(enableGpSupplierConfiguration);

            return services;
        }

        private static IServiceCollection RegisterBaseGpSystemsServices(this IServiceCollection services)
        {
            services.AddTransient<IHtmlSanitizer, HtmlSanitizer>();
            services.AddSingleton<IGpSystemFactory, GpSystemFactory>();
            services.AddSingleton<IIm1CacheService, Im1CacheService>();
            services.AddSingleton<IIm1CacheKeyGenerator, Im1CacheKeyGenerator>();

            return services;
        }

        private static IServiceCollection RegisterGpSystems(this IServiceCollection services, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            if (enableGpSupplierConfiguration.EnableEmis)
            {
                services.RegisterEmisServices();
            }

            if (enableGpSupplierConfiguration.EnableTpp)
            {
                services.RegisterTppServices();
            }

            if (enableGpSupplierConfiguration.EnableVision)
            {
                services.RegisterVisionServices();
            }
            
            if (enableGpSupplierConfiguration.EnableMicrotest)
            {
                services.RegisterMicrotestServices();
            }

            return services;
        }
    }
}
