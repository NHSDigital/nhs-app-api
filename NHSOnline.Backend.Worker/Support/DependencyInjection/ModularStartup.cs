using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.Support.DependencyInjection
{
    public sealed class ModularStartup
    {
        private readonly IEnumerable<IModule> _modules;
        private readonly IConfiguration _configuration;

        public ModularStartup(IConfiguration configuration)
        {
            _configuration = configuration ??
                             throw new ArgumentNullException(nameof(configuration));

            _modules = FindAllLoadedModules();
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

        private static IEnumerable<IModule> FindAllLoadedModules()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(x => x.GetTypes())
                .Where(t => typeof(IModule).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass)
                .Select(t => (IModule)Activator.CreateInstance(t))
                .ToList();
        }
    }
}