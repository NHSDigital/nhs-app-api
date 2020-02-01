using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp
{
    internal static class ServiceCollectionExtensions
    {
        internal static void ReplacePrimaryHttpMessageHandler<TClient, THandler>(
            this ServiceCollection services)
            where THandler : HttpMessageHandler
        { 
            var primaryHandler = services.First(IsPrimaryHandler);
            services.Remove(primaryHandler);

            services.AddTransient<IConfigureOptions<HttpClientFactoryOptions>>(serviceProvider =>
                new ConfigureNamedOptions<HttpClientFactoryOptions>(
                    typeof(TClient).Name,
                    options => options.HttpMessageHandlerBuilderActions.Add(builder => SetPrimaryHandler(builder, serviceProvider))));

            void SetPrimaryHandler(HttpMessageHandlerBuilder builder, IServiceProvider serviceProvider)
                => builder.PrimaryHandler = serviceProvider.GetRequiredService<THandler>();
        }

        private static bool IsPrimaryHandler(ServiceDescriptor descriptor)
        {
            if (descriptor.ServiceType == typeof(IConfigureOptions<HttpClientFactoryOptions>) &&
                descriptor.ImplementationFactory != null)
            {
                return descriptor.ImplementationFactory.Method.Name.StartsWith("<ConfigurePrimaryHttpMessageHandler>", StringComparison.Ordinal);
            }

            return false;
        }
    }
}