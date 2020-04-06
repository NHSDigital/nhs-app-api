using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public class ServiceConfigurationModule : Backend.Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITermsAndConditionsService, TermsAndConditionsService>();
            services.AddSingleton<ITermsAndConditionsRepository, TermsAndConditionsRepository>();
            services.AddSingleton(typeof(IApiMongoClient<>), typeof(ApiMongoClient<>));

            base.ConfigureServices(services, configuration);
        }
    }
}