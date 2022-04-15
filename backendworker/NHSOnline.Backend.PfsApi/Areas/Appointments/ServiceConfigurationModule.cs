using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Auth;
using NHSOnline.Backend.Metrics;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAppointmentSlotMetadataLogger, AppointmentSlotMetadataLogger>();
            services.AddTransient<IAppointmentTypeTransformingVisitor, AppointmentTypeTransformingVisitor>();
            services.AddTransient<IMetricContext, UserSessionMetricContext>();
            base.ConfigureServices(services, configuration);
        }
    }
}