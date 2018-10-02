using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Prescriptions
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<VisionPrescriptionService>();
            services.AddTransient<VisionPrescriptionRequestValidationService>();
            services.AddTransient<IVisionPrescriptionMapper, VisionPrescriptionMapper>();

            base.ConfigureServices(services, configuration);
        }
    }
}
