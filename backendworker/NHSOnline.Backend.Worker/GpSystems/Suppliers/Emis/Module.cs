using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class Module : Support.DependencyInjection.Module
    {
        private readonly ILoggerFactory _loggerFactory;

        public Module(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var defaultHttpTimeoutSeconds = configuration.ConfigurationSettings().GetOrWarn("DefaultHttpTimeoutSeconds",
                _loggerFactory.CreateLogger<Module>());
            
            services.AddHttpClient<EmisHttpClient>(client => 
            {
                client.Timeout = TimeSpan.FromSeconds(int.Parse(defaultHttpTimeoutSeconds));
            });
            
            services.AddSingleton<IGpSystem, EmisGpSystem>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();
            
            services.AddTransient<EmisTokenValidationService>();
            
            base.ConfigureServices(services, configuration);
        }
    }
}