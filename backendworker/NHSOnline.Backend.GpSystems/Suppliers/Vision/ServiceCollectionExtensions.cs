using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Appointments;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Prescriptions;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Session;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterVisionPfsServices(this IServiceCollection services)
        {
            services.RegisterVisionBaseServices();
            services.RegisterVisionAppointmentsServices();
            services.RegisterVisionEnvelopeServices();
            services.RegisterVisionPrescriptionsServices();
            services.RegisterVisionPatientRecordServices();
            services.RegisterVisionSessionServices();
            services.RegisterVisionPrescriptionsServices();
            services.RegisterVisionDemographicsServices();

            return services;
        }

        private static IServiceCollection RegisterVisionBaseServices(this IServiceCollection services)
        {
            services.AddSingleton<IGpSystem, VisionGpSystem>();
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

            return services;
        }

        public static IServiceCollection RegisterVisionCidServices(this IServiceCollection services)
        {
            services.RegisterVisionBaseServices();
            services.RegisterVisionEnvelopeServices();
            services.RegisterVisionLinkageServices();
            services.RegisterVisionIm1ConnectionServices();

            return services;
        }
    }
}