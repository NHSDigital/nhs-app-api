using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Api.Client;
using NHSOnline.App.Logging;

namespace NHSOnline.App.Api.Logging
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoggingServices(this IServiceCollection services)
        {
            return services
                .AddEndpoint<ApiCreateLogRequest, CreateLoggingMessageBuilder, ApiCreateLogResult, CreateLoggingResponseParser>()
                .AddTransient<ICloudLog, CloudLoggingService>();
        }
    }
}