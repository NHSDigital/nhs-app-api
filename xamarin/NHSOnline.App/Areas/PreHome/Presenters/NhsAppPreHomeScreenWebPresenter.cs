using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.LoggedOut;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Areas.PreHome.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Services;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.PreHome.Presenters
{
    internal class NhsAppPreHomeScreenWebPresenter
    {
        private bool _hasAlreadyAppeared;
        private Uri? _deeplinkUrl;

        private readonly INhsAppPreHomeScreenWebView _view;
        private readonly NhsAppPreHomeScreenWebModel _model;
        private readonly INhsAppWebConfiguration _config;
        private readonly IPageFactory _pageFactory;
        private readonly ILogger _logger;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INotifications _notifications;
        private readonly ICookieService _cookieService;
        private readonly IAlertDialog _alertDialog;

        private Uri? ResolveDeeplinkUrl => _deeplinkUrl ?? _model.DeeplinkUrl;

        public NhsAppPreHomeScreenWebPresenter(
            INhsAppPreHomeScreenWebView view,
            NhsAppPreHomeScreenWebModel model,
            INhsAppWebConfiguration config,
            ILogger<NhsAppPreHomeScreenWebPresenter> logger,
            IBrowserOverlay browserOverlay,
            IPageFactory pageFactory,
            INotifications notifications,
            ICookieService cookieService,
            IAlertDialog alertDialog)
        {
            _view = view;
            _model = model;
            _config = config;
            _logger = logger;
            _browserOverlay = browserOverlay;
            _pageFactory = pageFactory;
            _notifications = notifications;
            _cookieService = cookieService;
            _alertDialog = alertDialog;

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler<Uri>(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterHandler(GetNotificationsStatusRequested, (view, handler) => view.GetNotificationsStatusRequested = handler)
                .RegisterHandler(GoToLoggedInHomeRequested, (view, handler) => view.GoToLoggedInHomeRequested = handler)
                .RegisterHandler(LogoutRequested, (view, handler) => view.LogoutRequested = handler)
                .RegisterHandler<string>(RequestPnsToken, (view, handler) => view.GetPnsTokenRequested = handler)
                .RegisterHandler(ResetAndShowErrorRequested, (view, handler) => view.ResetAndShowErrorRequested = handler)
                .RegisterHandler(OnSessionExpiringRequested, (view, handler) => view.OnSessionExpiringRequested = handler)
                .RegisterHandler(SessionExpiredRequested, (view, handler) => view.SessionExpiredRequested = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeeplinkRequested = handler);
        }

        private Task OnSessionExpiringRequested()
        {
            _logger.LogInformation("Display session expiring warning");

            _alertDialog.DisplayAlertDialog(
                "For security reasons, we'll log you out of the NHS App in 1 minute.",
                "Stay logged in",
                "Log out",
                () => _view.SendSessionExtend().PreserveThreadContext(),
                () => _view.Logout().PreserveThreadContext()
            );

            return Task.CompletedTask;
        }

        private async Task SessionExpiredRequested()
        {
            _logger.LogInformation("{Method}", nameof(SessionExpiredRequested));

            var model = new LoggedOutHomeScreenModel(LoggedOutHomeScreenStates.SessionExpired);

            var page = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.PopToNewRoot(page).PreserveThreadContext();
        }

        private async Task GoToLoggedInHomeRequested()
        {
            var homePageModel = new NhsAppWebModel(ResolveDeeplinkUrl);
            var homePage = _pageFactory.CreatePageFor(homePageModel);

            await _view.AppNavigation.PopToNewRoot(homePage).PreserveThreadContext();
        }

        private async Task LogoutRequested()
        {
            _logger.LogInformation("{Method}", nameof(LogoutRequested));

            var model = new LoggedOutHomeScreenModel();
            var page = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.PopToNewRoot(page).PreserveThreadContext();
        }

        private async Task ViewOnAppearing()
        {
            if (_hasAlreadyAppeared)
            {
                return;
            }

            _hasAlreadyAppeared = true;

            foreach (var cookie in _model.CookieJar.Cookies)
            {
                await  _cookieService.SetCookie(cookie).PreserveThreadContext();
            }

            await DisplayNhsAppWeb().PreserveThreadContext();
        }

        private async Task ViewOnNavigating(WebNavigatingEventArgs args)
        {
            var uri = new Uri(args.Url);

            if (!IsNhsAppWeb(uri))
            {
                args.Cancel = true;
                await _browserOverlay
                    .OpenBrowserOverlay(uri)
                    .PreserveThreadContext();
            }
        }

        private bool IsNhsAppWeb(Uri uri)
        {
            return string.Equals(uri.Host, _config.Host, StringComparison.OrdinalIgnoreCase);
        }

        private Task ViewOnNavigationFailed(Uri failingUrl)
        {
            void RetryAction() => _view.GoToUri(failingUrl);

            var model = new PreHomeTryAgainNetworkErrorModel(RetryAction);
            var page = _pageFactory.CreatePageFor(model);
            return _view.AppNavigation.Push(page);
        }

        private async Task GetNotificationsStatusRequested()
        {
            var notificationStatus = await _notifications.GetDeviceNotificationsStatus().PreserveThreadContext();
            await _view.SendNotificationsStatus(notificationStatus.ToString()).PreserveThreadContext();
        }

        private async Task RequestPnsToken(string trigger)
        {
            var pnsTokenResult = await _notifications.GetPnsToken().PreserveThreadContext();

            if (pnsTokenResult is GetPnsTokenResult.Authorised authorisedResult)
            {
                var response = new NotificationAuthorisedResponse(
                    trigger,
                    authorisedResult.DevicePns,
                    authorisedResult.DeviceType);

                await _view.SendNotificationAuthorised(response).PreserveThreadContext();
            }
            else
            {
                await _view.SendNotificationUnauthorised().PreserveThreadContext();
            }
        }

        private async Task ResetAndShowErrorRequested()
        {
            await DisplayNhsAppWeb().PreserveThreadContext();
            //TODO ShowError
            _logger.LogInformation("Showing unexpected error");
        }

        private Task DisplayNhsAppWeb()
        {
            var homeUri = _config.PreHomeAddress;

            _view.GoToUri(homeUri);

            return Task.CompletedTask;
        }

        private Task DeeplinkRequested(Uri deeplinkUrl)
        {
            _deeplinkUrl = deeplinkUrl;
            return Task.CompletedTask;
        }
    }
}