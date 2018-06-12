using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Prescriptions
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmisPrescriptionMapper, EmisPrescriptionMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
