using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Users
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterFakeUsers(this IServiceCollection services)
        {
            var fakeUsers = FindAllFakeUsers();

            foreach (var fakeUserType in fakeUsers)
            {
                services.AddSingleton(typeof(IFakeUser), fakeUserType);
            }

            return services;
        }

        private static IEnumerable<Type> FindAllFakeUsers()
        {
            return typeof(FakeUser).Assembly.GetTypes()
                .Where(t => typeof(IFakeUser).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass);
        }
    }
}