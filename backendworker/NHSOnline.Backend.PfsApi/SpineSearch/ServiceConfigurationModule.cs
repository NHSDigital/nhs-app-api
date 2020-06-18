using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.PfsApi.SpineSearch
{
    public sealed class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            base.ConfigureServices(services, configuration);

            services.AddTransient<ILdapConnectionService, LdapConnectionService>();
            services.AddTransient<ISpineSearchService, SpineSearchService>();
        }
    }
}