using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp
{
    internal static class ServiceCollectionExtensions
    {
        internal static void AddConfig<TConfig>(this IServiceCollection services, string sectionName) where TConfig : class
        {
            services.AddTransient(ResolveConfig);

            TConfig ResolveConfig(IServiceProvider sp)
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var section = configuration.GetSection(sectionName);
                return section.Get<TConfig>() ?? throw new InvalidOperationException($"Missing configuration section {sectionName}");
            }
        }
    }
}