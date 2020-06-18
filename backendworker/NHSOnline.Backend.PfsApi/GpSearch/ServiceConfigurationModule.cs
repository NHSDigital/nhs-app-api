using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.PfsApi.GpSearch.Pharmacy;

namespace NHSOnline.Backend.PfsApi.GpSearch
{
    public class ServiceConfigurationModule: Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IGpLookupClient, GpLookupClient>();
            services.AddSingleton<IGpLookupConfig, GpLookupConfig>();
            services.AddTransient<IGpSearchService, GpSearchService>();
            services.AddHttpClient<GpLookupHttpClient>();
            services.AddTransient<INhsSearchResultChecker, NhsSearchResultChecker>();
            services.AddTransient<INhsSearchMapper, NhsSearchMapper>();
            services.AddTransient<IPostcodeParser, PostcodeParser>();
            services.AddTransient<GpSearchService>();
            services.AddTransient<IPharmacySearchService, PharmacySearchService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}