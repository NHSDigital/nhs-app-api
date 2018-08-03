using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Prescriptions
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<TppCourseService>();
            services.AddTransient<TppPrescriptionService>();
            services.AddTransient<TppPrescriptionRequestValidationService>();

            services.AddTransient<ITppCourseMapper, TppCourseMapper>();
            services.AddTransient<ITppPrescriptionMapper, TppPrescriptionMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
