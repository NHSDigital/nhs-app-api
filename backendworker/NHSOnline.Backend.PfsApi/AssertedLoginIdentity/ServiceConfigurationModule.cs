using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.PfsApi.AssertedLoginIdentity
{
    public class ServiceConfigurationModule: NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAssertedLoginIdentityService, AssertedLoginIdentityService>();

            base.ConfigureServices(services, configuration);
        }
    }
}