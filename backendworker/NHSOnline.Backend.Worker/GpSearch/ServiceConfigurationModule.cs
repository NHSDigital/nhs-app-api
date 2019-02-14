using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSearch
{
    public class ServiceConfigurationModule: NHSOnline.Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IGpLookupClient, GpLookupClient>();
            services.AddTransient<IGpLookupConfig, GpLookupConfig>();
            services.AddTransient<IGpSearchService, GpSearchService>();
            services.AddHttpClient<GpLookupHttpClient>();
            services.AddTransient<INhsSearchResultChecker, NhsSearchResultChecker>();
            services.AddTransient<INhsSearchMapper, NhsSearchMapper>();
            services.AddTransient<IPostcodeParser, PostcodeParser>();
            services.AddTransient<GpSearchService>();           
            
            base.ConfigureServices(services, configuration);
        }
    }
}