using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.DependencyInjection
{
    public sealed class ModularStartup
    {
        private readonly IEnumerable<IServiceConfigurationModule> _modules;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public ModularStartup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration ??
                             throw new ArgumentNullException(nameof(configuration));
            _loggerFactory = loggerFactory;

            _modules = FindAllEnabledModules();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            foreach (var module in _modules)
            {  
                module.ConfigureServices(services, _configuration);
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            foreach (var module in _modules)
            {
                module.Configure(app, env);
            }
        }

        private IEnumerable<IServiceConfigurationModule> FindAllEnabledModules()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(x => x.GetTypes())
                .Where(t => typeof(IServiceConfigurationModule).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass)
                .Select(t =>
                {
                    if (t
                        .GetConstructors()
                        .SelectMany(c => c.GetParameters())
                        .Any(p => p.ParameterType == typeof(ILoggerFactory)))
                    {
                        return (IServiceConfigurationModule) Activator.CreateInstance(t, _loggerFactory);    
                    }
                    
                    return (IServiceConfigurationModule) Activator.CreateInstance(t);
                })
                .Where(x => x.IsEnabled(_configuration))
                .ToList();
        }
    }
}