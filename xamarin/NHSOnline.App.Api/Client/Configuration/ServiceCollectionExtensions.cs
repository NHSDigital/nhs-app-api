using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.Api.Client.Configuration
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationEndpoints(this IServiceCollection services)
        {
            return services
                .AddEndpoint<ApiConfigurationRequest, GetConfigurationRequestMessageBuilder, ApiGetConfigurationResult, GetConfigurationResponseParser>()
                .AddTransient<IResponseModelValidator<GetConfigurationResponseModel, GetConfigurationResponse>, GetConfigurationResponseValidator>();
        }
    }
}