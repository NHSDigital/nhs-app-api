using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Api.Client.Session;

namespace NHSOnline.App.Api.Client
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services)
        {
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
    }
}
