using System;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Navigation;
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

            view.LoginRequested += ViewOnLoginRequested;
            view.NhsUkCovidAppPageRequested += LoadCovidUrl;
            view.BackRequested += BackRequested;
        }

        private async void ViewOnLoginRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Login Requested");

            _userPreferencesService.ShowGettingStarted = false;

            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes);

            var loginPage = _pageFactory.CreatePageFor(loginModel);
            await _view.Navigation.ReplaceCurrentPage(loginPage).PreserveThreadContext();
        }

        private async void LoadCovidUrl(object sender, EventArgs e)
        {
            _logger.LogInformation("Accessing covid url");
            await _browserOverlay.OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkCovidAppUrl)
                .PreserveThreadContext();
        }

        private async void BackRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Back Requested");
            await _view.Navigation.PopAsync().PreserveThreadContext();
        }
    }
}