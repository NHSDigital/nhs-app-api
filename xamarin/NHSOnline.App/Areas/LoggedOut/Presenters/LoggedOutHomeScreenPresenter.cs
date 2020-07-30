using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
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

        public LoggedOutHomeScreenPresenter(
            ILoggedOutHomeScreenView view,
            ILogger<LoggedOutHomeScreenPresenter> logger,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            INhsLoginService nhsLoginService,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration)
        {
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _nhsLoginService = nhsLoginService;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;

            view.LoginRequested += ViewOnLoginRequested;
            view.NhsUkCovidConditionsServicePageRequested += LoadCovidConditionsUrl;
            view.NhsUkLoginHelpServicePageRequested += LoadLoginHelpUrl;
            view.ResetAndShowErrorRequested += ResetAndShowErrorRequested;
        }

        private async void ViewOnLoginRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Login Requested");

            if (_userPreferencesService.ShowBeforeYouStart)
            {
                await ShowBeforeYouStartPage().PreserveThreadContext();
            }
            else
            {
                await ShowNhsLoginPage().PreserveThreadContext();
            }
        }

        private async Task ShowBeforeYouStartPage()
        {
            var beforeYouStart = new BeforeYouStartModel();
            var beforeYouStartPage = _pageFactory.CreatePageFor(beforeYouStart);
            await _view.Navigation.PushAsync(beforeYouStartPage).PreserveThreadContext();
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

        private void ResetAndShowErrorRequested(object sender, EventArgs e)
        {
            // TODO ShowError
            _logger.LogInformation($"Showing unexpected error");
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
