using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public class Module : Support.DependencyInjection.Module
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICitizenIdService, CitizenIdService>();
            services.AddScoped<ICitizenIdClient, CitizenIdClient>();
            services.AddScoped<ICitizenIdConfig, CitizenIdConfig>();
        }
    }
}
