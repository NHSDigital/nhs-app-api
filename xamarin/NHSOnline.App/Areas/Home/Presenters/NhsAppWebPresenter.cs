using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Api.Session;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.ThirdParty.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Navigation;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class NhsAppWebPresenter
    {
        private const string NhsOnlineSessionCookieName = "nhso.session";

        private readonly INhsAppWebView _view;
        private readonly NhsAppWebModel _model;
        private readonly INhsAppWebConfiguration _config;
        private readonly ILogger _logger;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IAppBrowserTab _appBrowserTab;
        private readonly IPageFactory _pageFactory;
        private readonly INhsAppNavigationHandler _navigationHandler;

        public NhsAppWebPresenter(
            INhsAppWebView view,
            NhsAppWebModel model,
            INhsAppWebConfiguration config,
            ILogger<NhsAppWebPresenter> logger,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IAppBrowserTab appBrowserTab,
            IPageFactory pageFactory)
        {
            _view = view;
            _model = model;
            _config = config;
            _logger = logger;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _appBrowserTab = appBrowserTab;
            _pageFactory = pageFactory;
            _navigationHandler = new NhsAppNavigationHandler(view);

            _view.Appearing += ViewOnAppearing;
            _view.SettingsRequested += _navigationHandler.SettingsRequested;
            _view.HomeRequested += _navigationHandler.HomeRequested;
            _view.HelpRequested += HelpRequested;
            _view.SymptomsRequested += _navigationHandler.SymptomsRequested;
            _view.AppointmentsRequested += _navigationHandler.AppointmentsRequested;
            _view.PrescriptionsRequested += _navigationHandler.PrescriptionsRequested;
            _view.RecordRequested += _navigationHandler.RecordRequested;
            _view.MoreRequested += _navigationHandler.MoreRequested;

            _view.NavigateToThirdPartyRequested += NavigateToThirdPartyRequested;
        }

        private async void NavigateToThirdPartyRequested(object sender, string url)
        {
            _logger.LogInformation("GoToThirdPartyRequested - {Url}", url);
            await NavigateToSilverWebPage(url).PreserveThreadContext();
        }

        private async Task NavigateToSilverWebPage(string url)
        {
            var silverWebModel = new NhsAppSilverWebModel(new NhsAppPopToRootNavigationHandler(_navigationHandler, _view.Navigation), url);

            var silverPage = _pageFactory.CreatePageFor(silverWebModel);
            await _view.Navigation.PushAsync(silverPage).PreserveThreadContext();
        }

        private async void ViewOnAppearing(object sender, EventArgs e)
        {
            _view.Appearing -= ViewOnAppearing;
            await DisplayNhsAppWeb().PreserveThreadContext();
        }

        private async void HelpRequested(object sender, EventArgs e)
        {
            await _appBrowserTab.OpenAppBrowserTab(
                _nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }

        private async Task DisplayNhsAppWeb()
        {
            var homeUri = _config.BaseAddress;

            await ConfigureCookies(homeUri).PreserveThreadContext();
            _view.GoToUri(homeUri);
        }

        private async Task ConfigureCookies(Uri homeUri)
        {
            await CopyCookiesFromModel(homeUri).PreserveThreadContext();

            await CreateNhsOnlineSessionCookie(homeUri).PreserveThreadContext();
        }

        private async Task CreateNhsOnlineSessionCookie(Uri homeUri)
        {
            var sessionCookieJson = JsonConvert.SerializeObject(
                new NhsOnlineSessionCookie(_model.UserSession),
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            var sessionCookieEscaped = Uri.EscapeDataString(sessionCookieJson);

            _logger.LogTrace("Creating cookie {CookieName}: {CookieValue}", NhsOnlineSessionCookieName, sessionCookieEscaped);

            await _view.AddCookie(new Cookie(NhsOnlineSessionCookieName, sessionCookieEscaped, "/", homeUri.Host)
            {
                Secure = _config.NhsOnlineSessionCookieSecure,
                HttpOnly = false
            }).PreserveThreadContext();
        }

        private async Task CopyCookiesFromModel(Uri homeUri)
        {
            foreach (var cookie in _model.Cookies.GetCookies(homeUri).Cast<Cookie>())
            {
                _logger.LogTrace("Copying cookie {CookieName}: {CookieValue}", cookie.Name, cookie.Value);

                await _view.AddCookie(cookie).PreserveThreadContext();
            }
        }

        // Shim to convert the user session object from the API to the slightly
        // different structure the Web stores the same information in its cookie
        private sealed class NhsOnlineSessionCookie
        {
            private readonly UserSession _userSession;

            public NhsOnlineSessionCookie(UserSession userSession)
            {
                _userSession = userSession;
            }

            public string? Name => _userSession.Name;
            public int DurationSeconds => _userSession.SessionTimeout;
            public string? GpOdsCode => _userSession.OdsCode;
            public string? Token => _userSession.Token;
            public string LastCalledAt => _userSession.LastCalledAt;
            public string? NhsNumber => _userSession.NhsNumber;
            public string? DateOfBirth => _userSession.DateOfBirth;
            public string? AccessToken => _userSession.AccessToken;
            public string? ProofLevel => _userSession.ProofLevel;
        }
    }
}
