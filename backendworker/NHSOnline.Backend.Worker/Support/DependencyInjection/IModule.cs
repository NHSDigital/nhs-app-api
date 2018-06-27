using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Support.DependencyInjection
{
    public interface IModule
    {
        void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration);

        void Configure(IApplicationBuilder app, IHostingEnvironment env);
    }
}