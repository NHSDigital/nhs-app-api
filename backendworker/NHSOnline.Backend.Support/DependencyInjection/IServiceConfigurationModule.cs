using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Support.DependencyInjection
{
    public interface IServiceConfigurationModule
    {
        bool IsEnabled(IConfiguration configuration);
        
        void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration);
        
        void Configure(IApplicationBuilder app, IHostingEnvironment env);
    }
}