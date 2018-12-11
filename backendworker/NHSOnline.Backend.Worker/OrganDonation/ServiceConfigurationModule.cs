using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<OrganDonationHttpRequestIdentifier>();

            services.AddSingleton<IOrganDonationClient, OrganDonationClient>();
            services.AddSingleton<IOrganDonationConfig, OrganDonationConfig>();

            services.AddHttpClient<OrganDonationHttpClient>()
                .AddHttpMessageHandler<HttpTimeoutHandler<OrganDonationHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<OrganDonationHttpRequestIdentifier>>();

            base.ConfigureServices(services, configuration);
        }
    }
}
