using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NHSOnline.Backend.Support.Configuration;

namespace NHSOnline.Backend.Repository
{
    public static class StartupExtensions
    {
        public static void RegisterRepository<TRecord, TConfiguration>(this IServiceCollection services)
            where TRecord : RepositoryRecord
            where TConfiguration: class, IRepositoryConfiguration, IValidatable

        {
            services.AddSingleton<TConfiguration>();
            services.AddTransient<IValidatable>(sp => sp.GetRequiredService<TConfiguration>());
            services.AddTransient<IRepository<TRecord>, MongoRepository<TConfiguration, TRecord>>();
            services
                .AddHealthChecks()
                .AddCheck<ApiMongoClientHealthCheck<TConfiguration>>(typeof(TConfiguration).Name, timeout: TimeSpan.FromSeconds(1));
            services.TryAddSingleton(typeof(IApiMongoClient<>), typeof(ApiMongoClient<>));
        }
    }
}