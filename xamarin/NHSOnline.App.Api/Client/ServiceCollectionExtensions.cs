using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Api.Client.Session;
using NHSOnline.App.Config;

namespace NHSOnline.App.Api.Client
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services)
        {
            services
                .AddHttpClient<ApiHttpClient>(ConfigureApiHttpClient)
                .ConfigurePrimaryHttpMessageHandler(ConfigurePrimaryHttpMessageHandler);

            return services
                .AddTransient(typeof(IApiClientEndpoint<,>), typeof(ApiClientEndpoint<,>))
                .AddTransient<JsonRequestContentSerialiser>()
                .AddTransient<JsonResponseParser>()
                .AddSingleton(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                .AddSessionEndpoints();
        }

        internal static IServiceCollection AddEndpoint<TRequest, TRequestBuilder, TResult, TResponseParser>(
            this IServiceCollection services)
            where TRequestBuilder: class, IApiClientRequestMessageBuilder<TRequest>
            where TResponseParser : class, IApiClientResponseParser<TResult>
        {
            return services
                .AddTransient<IApiClientRequestMessageBuilder<TRequest>, TRequestBuilder>()
                .AddTransient<IApiClientResponseParser<TResult>, TResponseParser>();
        }

        private static HttpMessageHandler ConfigurePrimaryHttpMessageHandler(IServiceProvider serviceProvider)
        {
            var configureHttpClient = serviceProvider.GetRequiredService<IPrimaryHttpMessageHandlerFactory>();
            return configureHttpClient.CreatePrimaryHttpMessageHandler();
        }

        private static void ConfigureApiHttpClient(IServiceProvider serviceProvider, HttpClient httpClient)
        {
            var configuration = serviceProvider.GetRequiredService<INhsAppApiConfiguration>();
            httpClient.BaseAddress = configuration.BaseAddress;
        }
    }
}
