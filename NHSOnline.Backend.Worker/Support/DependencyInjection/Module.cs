using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Support.DependencyInjection
{
    public abstract class Module : IModule
    {
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }
}
