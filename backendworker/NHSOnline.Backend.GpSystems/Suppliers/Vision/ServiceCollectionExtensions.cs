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
            services.RegisterVisionDemographicsServices();
            services.RegisterVisionEnvelopeServices();
            services.RegisterVisionPatientRecordServices();
            services.RegisterVisionPrescriptionsServices();
            services.RegisterVisionSessionServices();

            return services;
        }

        public static IServiceCollection RegisterVisionCidServices(this IServiceCollection services)
        {
            services.RegisterVisionBaseServices();

            services.RegisterVisionEnvelopeServices();
            services.RegisterVisionIm1ConnectionServices();
            services.RegisterVisionLinkageServices();

            return services;
        }

        private static IServiceCollection RegisterVisionBaseServices(this IServiceCollection services)
        {
            services.AddTransient<IGpSystem, VisionGpSystem>();
            services.AddTransient<IVisionLinkageConfig, VisionLinkageConfig>();
            services.AddTransient<IVisionPfsClient, VisionPfsClient>();
            services.AddTransient<VisionPfsClientRequestSender>();
            services.AddTransient<IVisionLinkageClient, VisionLinkageClient>();
            services.AddTransient<IVisionClient, VisionClient>();

            services.AddTransient<VisionTokenValidationService>();

            services.AddTransient<VisionPFSHttpRequestIdentifier>();
            services.AddTransient<VisionLinkageHttpRequestIdentifier>();
            services.AddTransient<VisionHttpClientHandler>();

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
    }
}