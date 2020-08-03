using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Api.Session;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls.WebViews.KnownServices;
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

            _view.Appearing = ViewOnAppearing;
            _view.HelpRequested = HelpRequested;
            _view.OpenWebIntegrationRequested = OpenWebIntegrationRequested;
            _view.ResetAndShowErrorRequested = ResetAndShowErrorRequested;

            _view.SettingsRequested = _navigationHandler.SettingsRequested;
            _view.HomeRequested = _navigationHandler.HomeRequested;
            _view.SymptomsRequested = _navigationHandler.SymptomsRequested;
            _view.AppointmentsRequested = _navigationHandler.AppointmentsRequested;
            _view.PrescriptionsRequested = _navigationHandler.PrescriptionsRequested;
            _view.RecordRequested = _navigationHandler.RecordRequested;
            _view.MoreRequested = _navigationHandler.MoreRequested;
        }

        private async Task OpenWebIntegrationRequested(OpenWebIntegrationRequest request)
        {
            _logger.LogInformation("Opening Web Integration - {Url} ({MenuTab})", request.Url, request.MenuTab);

            var popToRootNavigationHandler = new NhsAppPopToRootNavigationHandler(_navigationHandler, _view.Navigation);
            var model = new WebIntegrationModel(popToRootNavigationHandler, request.Url, request.MenuTab);

            var page = _pageFactory.CreatePageFor(model);
            await _view.Navigation
                .PushAsync(page)
                .PreserveThreadContext();
        }

        private async Task ResetAndShowErrorRequested()
        {
            await DisplayNhsAppWeb().PreserveThreadContext();
            //TODO ShowError
            _logger.LogInformation($"Showing unexpected error");
        }

        private async Task ViewOnAppearing()
        {
            _view.Appearing = null;
            await ConfigureNhsOnlineCookies().PreserveThreadContext();
            await DisplayNhsAppWeb().PreserveThreadContext();
        }

        private async Task HelpRequested()
        {
            await _appBrowserTab.OpenAppBrowserTab(
                _nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }

        private Task DisplayNhsAppWeb()
        {
            var homeUri = _config.BaseAddress;

            _view.GoToUri(homeUri);

            return Task.CompletedTask;
        }

        private async Task ConfigureNhsOnlineCookies()
        {
            var homeUri = _config.BaseAddress;

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
