using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ServiceConfigurationModule> _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            if (bool.TryParse(configuration.GetOrWarn("GP_PROVIDER_ENABLED_TPP", _logger), out bool enabled) && enabled)
            {
                var configValue = configuration.ConfigurationSettings().GetOrWarn("DefaultHttpTimeoutSeconds",
                    _logger);
                var timeout = int.Parse(configValue, CultureInfo.InvariantCulture);

                services.AddSingleton<TppHttpClientHandler>();

                services.AddHttpClient<TppHttpClient>(client =>
                    {
                        client.Timeout = TimeSpan.FromSeconds(timeout);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        return new TppHttpClientHandler(configuration, _loggerFactory.CreateLogger<TppHttpClientHandler>())
                            .ConfigureForwardProxy(configuration);
                    });

                services.AddSingleton<IGpSystem, TppGpSystem>();
                services.AddSingleton<ITppClient, TppClient>();
                services.AddSingleton<ITppConfig, TppConfig>();
                services.AddTransient<IListSlotsReplyMapper, ListSlotsReplyMapper>();
                services.AddTransient<ISessionMapper, SessionMapper>();
                services.AddSingleton<IAppointmentSlotResultBuilder, TppAppointmentSlotsResultBuilder>();
                services.AddTransient<IAppointmentsReplyMapper, AppointmentsReplyMapper>();
                services.AddTransient<IAppointmentMapper, AppointmentMapper>();
                services.AddSingleton<IAppointmentsResultBuilder, TppAppointmentsResultBuilder>();

                services.AddTransient<TppTokenValidationService>();
                services.AddTransient<TppDemographicsService>();
                services.AddTransient<TppPatientRecordService>();
                services.AddTransient<TppPrescriptionService>();
                services.AddTransient<TppCourseService>();
                services.AddTransient<TppAppointmentSlotsService>();
                services.AddTransient<TppAppointmentsService>();

                _logger.LogDebug("Tpp GP Service was successfully configured");
            }
            else
            {
                _logger.LogDebug("Tpp GP Service was not configured");
            }

            base.ConfigureServices(services, configuration);

        }
    }
}