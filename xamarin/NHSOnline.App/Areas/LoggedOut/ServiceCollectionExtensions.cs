using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Areas.LoggedOut.Presenters;
using NHSOnline.App.Areas.LoggedOut.Views;
using NHSOnline.App.DependencyInjection;

namespace NHSOnline.App.Areas.LoggedOut
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddLoggedOutArea(this IServiceCollection services)
        {
            return services
                .AddModelViewPresenter<GettingStartedModel, GettingStartedPage, GettingStartedPresenter>()
                .AddModelViewPresenter<CreateSessionErrorBadRequestModel, CreateSessionErrorBadRequestPage, CreateSessionErrorBadRequestPresenter>()
                .AddModelViewPresenter<CreateSessionErrorBadResponseFromUpstreamSystemModel, CreateSessionErrorBadResponseFromUpstreamSystemPage, CreateSessionErrorBadResponseFromUpstreamSystemPresenter>()
                .AddModelViewPresenter<CreateSessionErrorFailedAgeRequirementModel, CreateSessionErrorFailedAgeRequirementPage, CreateSessionErrorFailedAgeRequirementPresenter>()
                .AddModelViewPresenter<CreateSessionErrorFallbackModel, CreateSessionErrorFallbackPage, CreateSessionErrorFallbackPresenter>()
                .AddModelViewPresenter<CreateSessionErrorModel, CreateSessionErrorPage, CreateSessionErrorPresenter>()
                .AddModelViewPresenter<CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel, CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPage, CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPresenter>()
                .AddModelViewPresenter<CreateSessionErrorUpstreamSystemTimeoutModel, CreateSessionErrorUpstreamSystemTimeoutPage, CreateSessionErrorUpstreamSystemTimeoutPresenter>()
                .AddModelViewPresenter<CreateSessionModel, CreateSessionPage, CreateSessionPresenter>()
                .AddModelViewPresenter<LoggedOutHomeScreenModel, LoggedOutHomeScreenPage, LoggedOutHomeScreenPresenter>()
                .AddModelViewPresenter<NhsLoginErrorModel, NhsLoginErrorPage, NhsLoginErrorPresenter>()
                .AddModelViewPresenter<NhsLoginModel, NhsLoginPage, NhsLoginPresenter>();
        }
    }
}