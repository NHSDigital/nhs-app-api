using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Session;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEmisServices(this IServiceCollection services)
        {
            services.RegisterEmisBaseServices();
            services.RegisterEmisAppointmentsServices();
            services.RegisterEmisPrescriptionsServices();
            services.RegisterEmisPatientRecordServices();
            services.RegisterEmisDemographicsServices();
            services.RegisterEmisSessionServices();

            services.RegisterEmisCidServices();

            return services;
        }

        private static IServiceCollection RegisterEmisBaseServices(this IServiceCollection services)
        {
            services.AddSingleton<EmisHttpClientHandler>();
            services.AddTransient<EmisHttpRequestIdentifier>();

            services.AddSingleton<IGpSystem, EmisGpSystem>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();

            services.AddHttpClient<EmisHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<EmisHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<EmisHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<EmisHttpRequestIdentifier>>();

            services.AddTransient<IEmisEnumMapper, EmisEnumMapper>();

            services.AddTransient<EmisTokenValidationService>();

            return services;
        }

        private static IServiceCollection RegisterEmisCidServices(this IServiceCollection services)
        {
            services.RegisterEmisIm1ConnectionServices();
            services.RegisterEmisLinkageServices();

            return services;
        }
    }
}