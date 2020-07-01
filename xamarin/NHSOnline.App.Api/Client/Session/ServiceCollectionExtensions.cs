using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.Api.Client.Session
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSessionEndpoints(this IServiceCollection services)
        {
            return services
                .AddEndpoint<ApiCreateSessionRequest, CreateSessionRequestMessageBuilder, ApiCreateSessionResult, CreateSessionResponseParser>();
        }
    }
}