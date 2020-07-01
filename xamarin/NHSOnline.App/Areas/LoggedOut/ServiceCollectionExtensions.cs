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
                .AddModelViewPresenter<BeforeYouStartModel, BeforeYouStartPage, BeforeYouStartPresenter>()
                .AddModelViewPresenter<CreateSessionErrorModel, CreateSessionErrorPage, CreateSessionErrorPresenter>()
                .AddModelViewPresenter<CreateSessionModel, CreateSessionPage, CreateSessionPresenter>()
                .AddModelViewPresenter<LoggedOutHomeScreenModel, LoggedOutHomeScreenPage, LoggedOutHomeScreenPresenter>()
                .AddModelViewPresenter<NhsLoginErrorModel, NhsLoginErrorPage, NhsLoginErrorPresenter>()
                .AddModelViewPresenter<NhsLoginModel, NhsLoginPage, NhsLoginPresenter>();
        }
    }
}