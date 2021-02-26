using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

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

        public LoggedOutHomeScreenPresenter(
            ILoggedOutHomeScreenView view,
            ILogger<LoggedOutHomeScreenPresenter> logger,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            INhsLoginService nhsLoginService,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            ILifecycle lifecycle)
        {
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _nhsLoginService = nhsLoginService;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _lifecycle = lifecycle;

            view.LoginRequested += ViewOnLoginRequested;
            view.NhsUkCovidConditionsServicePageRequested += LoadCovidConditionsUrl;
            view.NhsUkLoginHelpServicePageRequested += LoadLoginHelpUrl;
            view.BackRequested += BackRequested;

            view.ResetAndShowErrorRequested = ResetAndShowErrorRequested;
        }

        private async void ViewOnLoginRequested(object sender, EventArgs e)
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

        private void BackRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Back Requested");
            _lifecycle.CloseApplication();
        }

        private async Task ShowGettingStartedPage()
        {
            var gettingStartedModel = new GettingStartedModel();
            var gettingStartedPage = _pageFactory.CreatePageFor(gettingStartedModel);
            await _view.Navigation.PushAsync(gettingStartedPage).PreserveThreadContext();
        }

        private async Task ShowNhsLoginPage()
        {
            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes);
            var loginView = _pageFactory.CreatePageFor(loginModel);
            await _view.Navigation.PushAsync(loginView).PreserveThreadContext();
        }

        private async void LoadCovidConditionsUrl(object sender, EventArgs e)
        {
            _logger.LogInformation("Accessing covid conditions url");
            await OpenAppTab(_nhsExternalServicesConfiguration.NhsUkCovidConditionsUrl).PreserveThreadContext();
        }

        private async void LoadLoginHelpUrl(object sender, EventArgs e)
        {
            _logger.LogInformation("Accessing login help url");
            await OpenAppTab(_nhsExternalServicesConfiguration.NhsUkLoginHelpUrl).PreserveThreadContext();
        }

        private Task ResetAndShowErrorRequested()
        {
            // TODO ShowError
            _logger.LogInformation("Showing unexpected error");
            return Task.CompletedTask;
        }

        private static async Task OpenAppTab(Uri requestedService)
        {
            await Browser.OpenAsync(requestedService, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                PreferredToolbarColor = (Color) Application.Current.Resources["NhsUkBlue"],
                PreferredControlColor = (Color) Application.Current.Resources["NhsUkWhite"]
            }).PreserveThreadContext();
        }
    }
}
