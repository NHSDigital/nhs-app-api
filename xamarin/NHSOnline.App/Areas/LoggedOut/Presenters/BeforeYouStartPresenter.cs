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
    internal sealed class BeforeYouStartPresenter
    {
        private readonly IBeforeYouStartView _view;
        private readonly ILogger<BeforeYouStartPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly INhsLoginService _nhsLoginService;
        private readonly IAppBrowserTab _appBrowserTab;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;

        public BeforeYouStartPresenter(
            IBeforeYouStartView view,
            ILogger<BeforeYouStartPresenter> logger,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            INhsLoginService nhsLoginService,
            IAppBrowserTab appBrowserTab,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration)
        {
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _nhsLoginService = nhsLoginService;
            _appBrowserTab = appBrowserTab;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;

            view.LoginRequested += ViewOnLoginRequested;
            view.NhsUkCovidServicePageRequested += LoadCovidUrl;
            view.NhsUkConditionsServicePageRequested += LoadConditionsUrl;
            view.NhsUkOneOneOneServicePageRequested += LoadOneOneOneUrl;
            view.BackRequested += BackRequested;
        }

        private async void ViewOnLoginRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Login Requested");

            _userPreferencesService.ShowBeforeYouStart = false;

            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes);

            var loginPage = _pageFactory.CreatePageFor(loginModel);
            await _view.Navigation.ReplaceCurrentPage(loginPage).PreserveThreadContext();
        }

        private async void LoadCovidUrl(object sender, EventArgs e)
        {
            _logger.LogInformation("Accessing covid url");
            await _appBrowserTab.OpenAppBrowserTab(_nhsExternalServicesConfiguration.NhsUkCovidUrl)
                .PreserveThreadContext();
        }
        private async void LoadConditionsUrl(object sender, EventArgs e)
        {
            _logger.LogInformation("Accessing conditions url");
            await _appBrowserTab.OpenAppBrowserTab(_nhsExternalServicesConfiguration.NhsUkConditionsUrl)
                .PreserveThreadContext();
        }
        private async void LoadOneOneOneUrl(object sender, EventArgs e)
        {
            _logger.LogInformation("Accessing 111 url");
            await _appBrowserTab.OpenAppBrowserTab(_nhsExternalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async void BackRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Back Requested");
            await _view.Navigation.PopAsync().PreserveThreadContext();
        }
    }
}