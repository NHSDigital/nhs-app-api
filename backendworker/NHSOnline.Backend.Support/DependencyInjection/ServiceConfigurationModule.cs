using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Support.DependencyInjection
{
    public abstract class ServiceConfigurationModule : IServiceConfigurationModule
    {
        public virtual bool IsEnabled(IConfiguration configuration)
        {
            return true;
        }

        public virtual void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration)
        {
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }
}
