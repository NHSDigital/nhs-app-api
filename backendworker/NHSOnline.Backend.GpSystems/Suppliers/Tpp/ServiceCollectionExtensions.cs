using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterTppPfsServices(this IServiceCollection services)
        {
            services.RegisterBaseServices();

            services.RegisterTppAppointmentsServices();
            services.RegisterTppDemographicsServices();
            services.RegisterTppLinkedAccountsServices();
            services.RegisterTppPatientPracticeMessagingServices();
            services.RegisterTppPatientRecordServices();
            services.RegisterTppPrescriptionsServices();
            services.RegisterTppSessionServices();

            return services;
        }

        public static IServiceCollection RegisterTppCidServices(this IServiceCollection services)
        {
            services.RegisterBaseServices();
            services.RegisterTppIm1ConnectionServices();
            services.RegisterTppLinkageServices();

            return services;
        }

        private static IServiceCollection RegisterBaseServices(this IServiceCollection services)
        {
            services.AddTransient<TppHttpClientHandler>();
            services.AddTransient<TppHttpRequestIdentifier>();

            services.AddTransient<IGpSystem, TppGpSystem>();

            services.AddHttpClient<TppHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<TppHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<TppHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<TppHttpRequestIdentifier>>();

            services.AddTransient<TppTokenValidationService>();
            services.AddTransient<TppClientRequestBuilder>();
            services.AddTransient<ITppClientRequestBuilder, TppClientRequestBuilder>();
            services.AddTransient<Func<TppClientRequestBuilder>>(provider => provider.GetRequiredService<TppClientRequestBuilder>);
            services.AddTransient<TppClientRequestExecutor>();
            services.AddTransient<ITppClientRequestExecutor, TppClientRequestExecutor>();

            services.AddScoped<TppClientRequestLock>();
            services.AddTransient<ITppClientRequestSender, TppClientRequestSerializer>();
            services.AddTransient<TppClientRequestSender>();

            ConfigureClientRequests(services);

            return services;
        }

        private static void ConfigureClientRequests(IServiceCollection services)
        {
            services.ConfigureUserClientRequests()
                .ConfigurePatientClientRequests()
                .ConfigureMedicationClientRequests()
                .ConfigureAppointmentClientRequests()
                .ConfigureTestResultClientRequests()
                .ConfigureDocumentClientRequests()
                .ConfigureIm1MessagingClientRequests();
        }
    }
}
