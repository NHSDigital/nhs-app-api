using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Cookies;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.PreHome.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Services;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.PreHome.Presenters
{
    internal class NhsAppPreHomeScreenWebPresenter
    {
        private readonly INhsAppPreHomeScreenWebView _view;
        private readonly NhsAppPreHomeScreenWebModel _model;
        private readonly INhsAppWebConfiguration _config;
        private readonly IPageFactory _pageFactory;
        private readonly ILogger _logger;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly ICookieHandler _cookieHandler;

        public NhsAppPreHomeScreenWebPresenter(
            INhsAppPreHomeScreenWebView view,
            NhsAppPreHomeScreenWebModel model,
            INhsAppWebConfiguration config,
            ILogger<NhsAppPreHomeScreenWebPresenter> logger,
            IBrowserOverlay browserOverlay, IPageFactory pageFactory,
            ICookieHandler cookieHandler)
        {
            _view = view;
            _model = model;
            _config = config;
            _logger = logger;
            _browserOverlay = browserOverlay;
            _pageFactory = pageFactory;
            _cookieHandler = cookieHandler;

            _view.Appearing = ViewOnAppearing;
            _view.Navigating = ViewOnNavigating;
            _view.Navigated = ViewOnNavigated;
            _view.GetNotificationsStatusRequested = GetNotificationsStatusRequested;
            _view.GoToLoggedInHomeRequested = GoToLoggedInHomeRequested;
            _view.ResetAndShowErrorRequested = ResetAndShowErrorRequested;
        }

        private async Task GoToLoggedInHomeRequested()
        {
            var homePageModel = new NhsAppWebModel();
            var homePage = _pageFactory.CreatePageFor(homePageModel);

            await _view.AppNavigation.PopToNewRoot(homePage).PreserveThreadContext();
        }

        private async Task ViewOnAppearing()
        {
            _view.Appearing = null;
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
            await _view.SendNotificationsStatus("notDetermined").PreserveThreadContext();
        }

        private async Task ResetAndShowErrorRequested()
        {
            await DisplayNhsAppWeb().PreserveThreadContext();
            //TODO ShowError
            _logger.LogInformation($"Showing unexpected error");
        }

        private Task DisplayNhsAppWeb()
        {
            var homeUri = _config.PreHomeAddress;

            _view.GoToUri(homeUri);

            return Task.CompletedTask;
        }
    }
}