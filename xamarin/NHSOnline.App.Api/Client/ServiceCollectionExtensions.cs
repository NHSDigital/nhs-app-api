using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Api.Client.Configuration;
using NHSOnline.App.Api.Client.Errors;
using NHSOnline.App.Api.Client.Session;
using NHSOnline.App.Config;

namespace NHSOnline.App.Api.Client
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddClientServices(this IServiceCollection services, string nhsAppUserAgent)
        {
            services
                .AddHttpClient<ApiHttpClient>((serviceProvider, client)
                    => ConfigureApiHttpClient(serviceProvider, client, nhsAppUserAgent))
                .ConfigurePrimaryHttpMessageHandler(ConfigurePrimaryHttpMessageHandler);

            return services
                .AddTransient(typeof(IApiClientEndpoint<,>), typeof(ApiClientEndpoint<,>))
                .AddTransient<JsonRequestContentSerialiser>()
                .AddTransient<JsonResponseParser>()
                .AddSingleton(new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()})
                .AddSessionEndpoints()
                .AddConfigurationEndpoints()
                .AddErrors();
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

        private static void ConfigureApiHttpClient(IServiceProvider serviceProvider, HttpClient httpClient, string nhsAppUserAgent)
        {
            var configuration = serviceProvider.GetRequiredService<INhsAppApiConfiguration>();
            httpClient.BaseAddress = configuration.BaseAddress;
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(nhsAppUserAgent);
        }
    }
}
