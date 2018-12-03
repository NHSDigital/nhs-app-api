using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ServiceConfigurationModule> _logger;
        
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        protected override Supplier Supplier => Supplier.Tpp;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<TppHttpClientHandler>();
            services.AddTransient<TppHttpRequestIdentifier>();

            services.AddSingleton<IGpSystem, TppGpSystem>();
            services.AddSingleton<ITppClient, TppClient>();
            services.AddSingleton<ITppConfig, TppConfig>();
            services.AddTransient<IListSlotsReplyMapper, ListSlotsReplyMapper>();
            services.AddTransient<ISessionMapper, SessionMapper>();
            services.AddSingleton<IAppointmentSlotResultBuilder, TppAppointmentSlotsResultBuilder>();
            services.AddTransient<IAppointmentsReplyMapper, AppointmentsReplyMapper>();
            services.AddTransient<IAppointmentMapper, AppointmentMapper>();
            services.AddSingleton<IAppointmentsResultBuilder, TppAppointmentsResultBuilder>();

            services.AddHttpClient<TppHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<TppHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<TppHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<TppHttpRequestIdentifier>>();

            services.AddTransient<TppTokenValidationService>();
            services.AddTransient<TppDemographicsService>();
            services.AddTransient<TppPatientRecordService>();
            services.AddTransient<TppPrescriptionService>();
            services.AddTransient<TppCourseService>();
            services.AddTransient<TppAppointmentSlotsService>();
            services.AddTransient<TppAppointmentsService>();

            base.ConfigureServices(services, configuration);
        }
    }
}