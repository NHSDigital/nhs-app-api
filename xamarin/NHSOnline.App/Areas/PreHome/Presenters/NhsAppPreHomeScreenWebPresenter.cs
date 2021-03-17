using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Services.Cookies;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.PreHome.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Services;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.PreHome.Presenters
{
    internal class NhsAppPreHomeScreenWebPresenter
    {
        private bool _hasAlreadyAppeared;

        private readonly INhsAppPreHomeScreenWebView _view;
        private readonly NhsAppPreHomeScreenWebModel _model;
        private readonly INhsAppWebConfiguration _config;
        private readonly IPageFactory _pageFactory;
        private readonly ILogger _logger;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly ICookieHandler _cookieHandler;
        private readonly INotifications _notifications;

        public NhsAppPreHomeScreenWebPresenter(
            INhsAppPreHomeScreenWebView view,
            NhsAppPreHomeScreenWebModel model,
            INhsAppWebConfiguration config,
            ILogger<NhsAppPreHomeScreenWebPresenter> logger,
            IBrowserOverlay browserOverlay,
            IPageFactory pageFactory,
            ICookieHandler cookieHandler,
            INotifications notifications)
        {
            _view = view;
            _model = model;
            _config = config;
            _logger = logger;
            _browserOverlay = browserOverlay;
            _pageFactory = pageFactory;
            _cookieHandler = cookieHandler;
            _notifications = notifications;

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler<WebNavigatedEventArgs>(ViewOnNavigated, (view, handler) => view.Navigated = handler)
                .RegisterHandler(GetNotificationsStatusRequested, (view, handler) => view.GetNotificationsStatusRequested = handler)
                .RegisterHandler(GoToLoggedInHomeRequested, (view, handler) => view.GoToLoggedInHomeRequested = handler)
                .RegisterHandler<string>(RequestPnsToken, (view, handler) => view.GetPnsTokenRequested = handler)
                .RegisterHandler(ResetAndShowErrorRequested, (view, handler) => view.ResetAndShowErrorRequested = handler);
        }

        private async Task GoToLoggedInHomeRequested()
        {
            var homePageModel = new NhsAppWebModel();
            var homePage = _pageFactory.CreatePageFor(homePageModel);

            await _view.AppNavigation.PopToNewRootAnimated(homePage).PreserveThreadContext();
        }

        private async Task ViewOnAppearing()
        {
            if (_hasAlreadyAppeared)
            {
                return;
            }

            _hasAlreadyAppeared = true;

            await _cookieHandler.AddCookies(_view, _config.BaseAddress, _model.Cookies).PreserveThreadContext();
            await DisplayNhsAppWeb().PreserveThreadContext();
        }

        private async Task ViewOnNavigating(WebNavigatingEventArgs args)
        {
            _logger.LogInformation("Navigating: {Uri}", args.Url);
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

        private Task ViewOnNavigated(WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);
            return Task.CompletedTask;
        }

        private async Task GetNotificationsStatusRequested()
        {
            await _view.SendNotificationsStatus(_notifications.GetDeviceNotificationsStatus().ToString()).PreserveThreadContext();
        }

        private async Task RequestPnsToken(string trigger)
        {
            var pnsTokenResult = _notifications.GetPnsToken();

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
    }
}