using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Fake;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Vision;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.GpSystems
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterPfsGpSystemsServices(this IServiceCollection services, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            services.RegisterBaseGpSystemsServices();
            services.RegisterEmisPfsServices();
            services.RegisterTppPfsServices();

            services.RegisterVisionPfsServices();

            services.RegisterMicrotestPfsServices();

            if (enableGpSupplierConfiguration.EnableFake)
            {
                services.RegisterFakePfsServices();
            }

            return services;
        }

        public static IServiceCollection RegisterCidGpSystemsServices(this IServiceCollection services, EnableGpSupplierConfiguration enableGpSupplierConfiguration)
        {
            services.RegisterBaseGpSystemsServices();
            services.RegisterEmisCidServices();
            services.RegisterTppCidServices();

            services.RegisterVisionCidServices();

            services.RegisterMicrotestCidServices();

            if (enableGpSupplierConfiguration.EnableFake)
            {
                services.RegisterFakeCidServices();
            }

            return services;
        }

        private static IServiceCollection RegisterBaseGpSystemsServices(this IServiceCollection services)
        {
            services.AddTransient<IHtmlSanitizer, HtmlSanitizer>();
            services.AddTransient<ICancellationReasonService, CancellationReasonService>();
            services.AddTransient<IGpSystemFactory, GpSystemFactory>();
            services.AddTransient<IGpSystem, NullGpSystem>();
            services.AddTransient<IIm1CacheService, Im1CacheService>();
            services.AddTransient<IIm1CacheKeyGenerator, Im1CacheKeyGenerator>();
            services.AddTransient<IAppointmentCancellationReasonLogger, AppointmentCancellationReasonLogger>();

            services.AddTransient<IIm1ConnectionErrorCodes, Im1ConnectionErrorCodes>();

            return services;
        }
    }
}
