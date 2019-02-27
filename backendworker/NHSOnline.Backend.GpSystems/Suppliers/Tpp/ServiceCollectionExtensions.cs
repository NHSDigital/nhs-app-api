using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppServices(this IServiceCollection services)
        {
            services.RegisterTppBaseServices();
            services.RegisterTppPrescriptionsServices();
            services.RegisterTppAppointmentsServices();
            services.RegisterTppDemographicsServices();
            services.RegisterTppPatientRecordServices();
            services.RegisterTppSessionServices();

            services.RegisterTppCidServices();

            return services;
        }

        private static IServiceCollection RegisterTppBaseServices(this IServiceCollection services)
        {
            services.AddSingleton<TppHttpClientHandler>();
            services.AddTransient<TppHttpRequestIdentifier>();

            services.AddSingleton<IGpSystem, TppGpSystem>();
            services.AddSingleton<ITppClient, TppClient>();
            services.AddSingleton<ITppConfig, TppConfig>();

            services.AddHttpClient<TppHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<TppHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<TppHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<TppHttpRequestIdentifier>>();

            services.AddTransient<TppTokenValidationService>();

            return services;
        }

        private static IServiceCollection RegisterTppCidServices(this IServiceCollection services)
        {
            services.RegisterTppLinkageServices();
            services.RegisterTppIm1ConnectionServices();

            return services;
        }
    }
}