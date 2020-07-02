using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakeUsers(this IServiceCollection services)
        {
            services.RegisterRepository<FakeUser, FakeGpUserRepoConfiguration>();
            services.AddTransient<IFakeUserRepository, FakeUserRepository>();

            return services;
        }
    }
}
