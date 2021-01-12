using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    public static class Im1ConnectionCacheServiceCollectionExtensions
    {
        public static void AddIm1CacheService(this IServiceCollection services)
        {
            services.AddTransient<IIm1CacheService, Im1CacheService>();
            services.AddTransient<Im1TokenSerialiserService>();
            services.AddTransient<Im1TokenEncryptionService>();
            services.AddTransient<IMongoIm1Cache, MongoIm1Cache>();
            services.RegisterRepository<Im1CacheRecord, Im1CacheConfig>();
        }
    }
}