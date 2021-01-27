using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.App.Api.Client.Errors
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddErrors(this IServiceCollection services)
        {
            return services
                .AddTransient<IResponseModelValidator<PfsErrorResponseModel, PfsErrorResponse>, PfsErrorResponseValidator>();
        }
    }
}