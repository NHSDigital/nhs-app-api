using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.SessionManager
{
    public static class SessionManagerServiceCollectionExtensions
    {
        public static void AddMongoSessionCacheService(this IServiceCollection services)
        {
            services.AddSingleton<IMongoSessionCacheServiceConfig, MongoSessionCacheServiceConfig>();
            services.AddTransient<ISessionCacheService, MongoSessionCacheService>();
            services.AddTransient<UserSessionEncryptionService>();
            services.AddTransient<UserSessionSerialiserService>();
        }
    }
}
