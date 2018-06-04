using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPrescriptionRequestValidationService, PrescriptionRequestValidationService>();

            base.ConfigureServices(services, configuration);
        }
    }
}
