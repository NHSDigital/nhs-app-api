using System;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.DependencyInjection
{
    public static class NhsAppDependencyInjection
    {
        public static IServiceProvider Init(Action<IServiceCollection> configureServices)
        {
            var services = new ServiceCollection()
                .AddPageFactory();

            configureServices(services);

            var serviceProviderOptions = new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            };

            return services.BuildServiceProvider(serviceProviderOptions);
        }
    }
}
