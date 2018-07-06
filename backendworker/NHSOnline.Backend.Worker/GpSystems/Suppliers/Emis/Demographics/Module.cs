using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EmisDemographicsService>();
            
            services.AddTransient<IEmisDemographicsMapper, EmisDemographicsMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
