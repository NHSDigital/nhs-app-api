using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<EmisLinkageService>();
            
            services.AddTransient<IEmisLinkageMapper, EmisLinkageMapper>();
            base.ConfigureServices(services, configuration);
        }
    }
}
