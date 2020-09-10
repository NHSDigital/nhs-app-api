using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Support.Mappers
{
    public class ServiceConfigurationModule : DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMapper<string, ProofLevel?>, ProofLevelMapper>();

            base.ConfigureServices(services, configuration);
        }
    }
}
