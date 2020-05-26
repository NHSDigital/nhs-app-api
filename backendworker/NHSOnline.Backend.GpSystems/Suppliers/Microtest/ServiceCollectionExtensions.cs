using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Session;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterMicrotestPfsServices(this IServiceCollection services)
        {
            services.RegisterMicrotestBaseServices();

            services.RegisterMicrotestAppointmentsServices();
            services.RegisterMicrotestDemographicsServices();
            services.RegisterMicrotestPatientRecordServices();
            services.RegisterMicrotestPrescriptionsServices();
            services.RegisterMicrotestSessionServices();

            return services;
        }

        public static IServiceCollection RegisterMicrotestCidServices(this IServiceCollection services)
        {
            services.RegisterMicrotestBaseServices();

            services.RegisterMicrotestIm1ConnectionServices();
            services.RegisterMicrotestLinkageServices();

            return services;
        }

        private static IServiceCollection RegisterMicrotestBaseServices(this IServiceCollection services)
        {
            services.AddSingleton<MicrotestHttpClientHandler>();
            services.AddTransient<MicrotestHttpRequestIdentifier>();

            services.AddHttpClient<MicrotestHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<MicrotestHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<MicrotestHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<MicrotestHttpRequestIdentifier>>();

            services.AddSingleton<IGpSystem, MicrotestGpSystem>();
            services.AddSingleton<IMicrotestClient, MicrotestClient>();
            services.AddTransient<MicrotestTokenValidationService>();
            services.AddTransient<IMicrotestEnumMapper, MicrotestEnumMapper>();

            return services;
        }
    }
}