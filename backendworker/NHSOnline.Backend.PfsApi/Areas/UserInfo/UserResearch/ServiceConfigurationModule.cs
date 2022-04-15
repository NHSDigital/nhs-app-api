using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.PfsApi.Areas.UserInfo.UserResearch
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IUserResearchService, UserResearchService>();

            base.ConfigureServices(services, configuration);
        }
    }
}