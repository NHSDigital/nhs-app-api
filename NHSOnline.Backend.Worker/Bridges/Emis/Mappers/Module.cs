using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Mappers
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
