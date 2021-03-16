using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class GettingStartedPresenter
    {
        private readonly IGettingStartedView _view;
        private readonly ILogger<GettingStartedPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly INhsLoginService _nhsLoginService;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;

        public GettingStartedPresenter(
            IGettingStartedView view,
            ILogger<GettingStartedPresenter> logger,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            INhsLoginService nhsLoginService,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration)
        {
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _nhsLoginService = nhsLoginService;
            _browserOverlay = browserOverlay;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;

            view.AppNavigation
                .RegisterHandler(ViewOnLoginRequested, (view, handler) => view.LoginRequested = handler)
                .RegisterHandler(LoadCovidUrl, (view, handler) => view.NhsUkCovidAppPageRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler);
        }

        private async Task ViewOnLoginRequested()
        {
            _logger.LogInformation("Login Requested");

            _userPreferencesService.ShowGettingStarted = false;

            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes);

            var loginPage = _pageFactory.CreatePageFor(loginModel);
            await _view.AppNavigation.ReplaceCurrentPage(loginPage).PreserveThreadContext();
        }

        private async Task LoadCovidUrl()
        {
            _logger.LogInformation("Accessing covid url");
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkCovidAppUrl)
                .PreserveThreadContext();
        }

        private async Task BackRequested()
        {
            _logger.LogInformation("Back Requested");
            await _view.AppNavigation.Pop().PreserveThreadContext();
        }
    }
}