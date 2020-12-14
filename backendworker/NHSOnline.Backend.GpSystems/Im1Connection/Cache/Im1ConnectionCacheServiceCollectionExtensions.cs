using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    public static class Im1ConnectionCacheServiceCollectionExtensions
    {
        public static void AddIm1CacheService(this IServiceCollection services)
        {
            services.AddTransient<IIm1CacheService, Im1CacheService>();
            services.AddTransient<Im1TokenSerialiserService>();
            services.AddTransient<Im1TokenEncryptionService>();
            services.AddTransient<IMongoIm1Cache, DualMongoIm1Cache>();
        }
    }
}