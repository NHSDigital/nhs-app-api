using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class LoggedOutHomeScreenPresenter
    {
        private readonly ILoggedOutHomeScreenView _view;
        private readonly ILogger _logger;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly INhsLoginService _nhsLoginService;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly ILifecycle _lifecycle;
        private readonly IBrowserOverlay _browserOverlay;

        public LoggedOutHomeScreenPresenter(
            ILoggedOutHomeScreenView view,
            ILogger<LoggedOutHomeScreenPresenter> logger,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            INhsLoginService nhsLoginService,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            ILifecycle lifecycle,
            IBrowserOverlay browserOverlay)
        {
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _nhsLoginService = nhsLoginService;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _lifecycle = lifecycle;
            _browserOverlay = browserOverlay;

            _view.AppNavigation
                .RegisterHandler(ViewOnLoginRequested, (view, handler) => view.LoginRequested = handler)
                .RegisterHandler(LoadCovidConditionsUrl, (view, handler) => view.NhsUkCovidConditionsServicePageRequested = handler)
                .RegisterHandler(LoadLoginHelpUrl, (view, handler) => view.NhsUkLoginHelpServicePageRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler(ResetAndShowErrorRequested, (view, handler) => view.ResetAndShowErrorRequested = handler);
        }

        private async Task ViewOnLoginRequested()
        {
            _logger.LogInformation("Login Requested");

            if (_userPreferencesService.ShowGettingStarted)
            {
                await ShowGettingStartedPage().PreserveThreadContext();
            }
            else
            {
                await ShowNhsLoginPage().PreserveThreadContext();
            }
        }

        private Task BackRequested()
        {
            _logger.LogInformation("Back Requested");
            _lifecycle.CloseApplication();
            return Task.CompletedTask;
        }

        private async Task ShowGettingStartedPage()
        {
            var gettingStartedModel = new GettingStartedModel();
            var gettingStartedPage = _pageFactory.CreatePageFor(gettingStartedModel);
            await _view.AppNavigation.Push(gettingStartedPage).PreserveThreadContext();
        }

        private async Task ShowNhsLoginPage()
        {
            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes);
            var loginView = _pageFactory.CreatePageFor(loginModel);
            await _view.AppNavigation.Push(loginView).PreserveThreadContext();
        }

        private async Task LoadCovidConditionsUrl()
        {
            _logger.LogInformation("Accessing covid conditions url");
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkCovidConditionsUrl)
                .PreserveThreadContext();
        }

        private async Task LoadLoginHelpUrl()
        {
            _logger.LogInformation("Accessing login help url");
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkLoginHelpUrl)
                .PreserveThreadContext();
        }

        private Task ResetAndShowErrorRequested()
        {
            // TODO ShowError
            _logger.LogInformation("Showing unexpected error");
            return Task.CompletedTask;
        }
    }
}
