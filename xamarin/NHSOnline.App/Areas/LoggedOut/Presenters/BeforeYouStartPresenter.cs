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
    internal sealed class BeforeYouStartPresenter
    {
        private readonly IBeforeYouStartView _view;
        private readonly ILogger<BeforeYouStartPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly INhsLoginService _nhsLoginService;
        private readonly IBeforeYouStartConfiguration _beforeYouStartConfigurations;

        public BeforeYouStartPresenter(
            IBeforeYouStartView view,
            ILogger<BeforeYouStartPresenter> logger,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            INhsLoginService nhsLoginService,
            IBeforeYouStartConfiguration beforeYouStartConfig)
        {
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _nhsLoginService = nhsLoginService;
            _beforeYouStartConfigurations = beforeYouStartConfig;

            view.LoginRequested += ViewOnLoginRequested;
            view.NhsUkCovidServicePageRequested += LoadCovidUrl;
            view.NhsUkConditionsServicePageRequested += LoadConditionsUrl;
            view.NhsUkOneOneOneServicePageRequested += LoadOneOneOneUrl;
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
            await OpenAppTab(_beforeYouStartConfigurations.NhsUkCovidUrl).ConfigureAwait(false);
        }
        private async void LoadConditionsUrl(object sender, EventArgs e)
        {
            _logger.LogInformation("Accessing conditions url");
            await OpenAppTab(_beforeYouStartConfigurations.NhsUkConditionsUrl).ConfigureAwait(false);
        }
        private async void LoadOneOneOneUrl(object sender, EventArgs e)
        {
            _logger.LogInformation("Accessing 111 url");
            await OpenAppTab(_beforeYouStartConfigurations.OneOneOneUrl).ConfigureAwait(false);
        }

        private static async Task OpenAppTab(Uri requestedService)
        {
            await Browser.OpenAsync(requestedService, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                PreferredToolbarColor = (Color) Application.Current.Resources["NhsUkBlue"],
                PreferredControlColor = (Color) Application.Current.Resources["NhsUkWhite"]
            }).ConfigureAwait(false);
        }
    }
}