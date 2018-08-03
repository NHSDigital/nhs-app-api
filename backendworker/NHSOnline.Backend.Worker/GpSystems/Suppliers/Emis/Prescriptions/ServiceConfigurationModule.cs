using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EmisCourseService>();
            services.AddTransient<EmisPrescriptionService>();
            services.AddTransient<EmisPrescriptionRequestValidationService>();

            services.AddTransient<IEmisPrescriptionMapper, EmisPrescriptionMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
