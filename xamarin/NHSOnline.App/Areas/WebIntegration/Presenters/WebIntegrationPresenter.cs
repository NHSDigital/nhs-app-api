using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal sealed class WebIntegrationPresenter
    {
        private bool _hasAlreadyAppeared;

        private readonly IWebIntegrationView _view;
        private readonly WebIntegrationModel _model;
        private readonly IBrowser _browser;
        private readonly ILogger _logger;
        private readonly WebIntegrationUriDestination _uriDestination;
        private readonly SingleSignOnMonitor _singleSignOnMonitor;
        private readonly ICalendar _calendar;
        private readonly IFileHandler _fileHandler;
        private readonly IPageFactory _pageFactory;
        private readonly IDialogPresenter _dialogPresenter;
        private readonly FileDownloadService _fileDownloadService;

        public WebIntegrationPresenter(
            IWebIntegrationView webIntegrationView,
            WebIntegrationModel model,
            INhsLoginConfiguration nhsLoginConfiguration,
            IBrowser browser,
            ILogger<WebIntegrationPresenter> logger,
            ICalendar calendar,
            IFileHandler fileHandler,
            IPageFactory pageFactory,
            IDialogPresenter dialogPresenter,
            FileDownloadService fileDownloadService)
        {
            _view = webIntegrationView;
            _model = model;
            _browser = browser;
            _logger = logger;
            _calendar = calendar;
            _fileHandler = fileHandler;
            _pageFactory = pageFactory;
            _dialogPresenter = dialogPresenter;
            _fileDownloadService = fileDownloadService;

            _uriDestination = new WebIntegrationUriDestination(nhsLoginConfiguration, model.WebIntegrationRequest.Url, model.AdditionalDomains);
            _singleSignOnMonitor = new SingleSignOnMonitor(nhsLoginConfiguration, logger);

            _view.SetNavigationFooterItem(model.FooterItem);
            _view.SetWebIntegrationRequest(model.WebIntegrationRequest);

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler(HelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterHandler(ViewOnSslError, (view, handler) => view.SslError = handler)
                .RegisterHandler<WebViewPageLoadEventArgs>(ViewOnPageLoadComplete, (view, handler) => view.PageLoadComplete = handler)
                .RegisterHandler(model.NavigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler)
                .RegisterHandler(model.NavigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(model.NavigationHandler.AdviceRequested, (view, handler) => view.AdviceRequested = handler)
                .RegisterHandler(model.NavigationHandler.AppointmentsRequested, (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(model.NavigationHandler.PrescriptionsRequested, (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(model.NavigationHandler.YourHealthRequested, (view, handler) => view.YourHealthRequested = handler)
                .RegisterHandler(model.NavigationHandler.MessagesRequested, (view, handler) => view.MessagesRequested = handler)
                .RegisterHandler<string>(GoToNhsAppPageRequested, (view, handler) => view.GoToNhsAppPageRequested = handler)
                .RegisterHandler<Uri>(OpenBrowserOverlayRequested, (view, handler) => view.OpenBrowserOverlayRequested = handler)
                .RegisterHandler<string>(OpenExternalBrowserRequested, (view, handler) => view.OpenExternalBrowserRequested = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeepLinkRequested = handler)
                .RegisterHandler<AddEventToCalendarRequest>(AddEventToCalendarRequested, (view, handler) => view.AddEventToCalendarRequested = handler)
                .RegisterHandler<DownloadRequest>(StartDownloadRequested, (view, handler) => view.StartDownloadRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler);
        }

        private async Task GoToNhsAppPageRequested(string page)
        {
            _logger.LogInformation("Going to NHS App Page - {page}", page);

            await _model.NavigationHandler.GoToNhsAppPageRequested(page).PreserveThreadContext();
        }

        private async Task OpenBrowserOverlayRequested(Uri overlayUri)
        {
            await _browser
                .OpenBrowserOverlay(overlayUri)
                .PreserveThreadContext();
        }

        private async Task OpenExternalBrowserRequested(string externalUri)
        {
            await _browser
                .OpenExternalBrowser(new Uri(externalUri))
                .PreserveThreadContext();
        }

        private async Task HelpRequested()
        {
            await _browser
                .OpenBrowserOverlay(_model.HelpUrl)
                .PreserveThreadContext();
        }

        private async Task StartDownloadRequested(DownloadRequest downloadRequest)
        {
            await _fileDownloadService.StartDownloadRequested(_dialogPresenter, downloadRequest, _fileHandler,
                _view.GetWebViewElement(),
                async () =>
                {
                    var model = new FullNavigationTryAgainFileDownloadErrorModel(_model.NavigationHandler,
                        _model.FooterItem, _model.HelpUrl);
                    var page = _pageFactory.CreatePageFor(model);
                    await _view.AppNavigation.Push(page).PreserveThreadContext();
                }).PreserveThreadContext();
        }

        private async Task DeeplinkRequested(Uri deepLinkUrl)
        {
            _logger.LogInformation("Redirecting to deeplink - {deeplink}", deepLinkUrl);

            await _model.NavigationHandler.RedirectToDeepLinkRequested(deepLinkUrl).PreserveThreadContext();
        }

        private Task ViewOnAppearing()
        {
            if (_hasAlreadyAppeared)
            {
                return Task.CompletedTask;
            }

            _hasAlreadyAppeared = true;

            return Task.CompletedTask;
        }

        private void ViewOnNavigating(WebNavigatingEventArgs webNavigatingEventArgs)
        {
            var url = new Uri(webNavigatingEventArgs.Url);
            if (_uriDestination.ShouldOpenInBrowserOverlay(url))
            {
                webNavigatingEventArgs.Cancel = true;
                NhsAppResilience.ExecuteOnMainThread(() =>
                {
                    _browser.OpenBrowserOverlay(url).PreserveThreadContext();
                });
            }
        }

        private Task ViewOnNavigationFailed()
        {
            var model = new FullNavigationBackToHomeNetworkErrorModel(_model.NavigationHandler, _model.FooterItem);
            var page = _pageFactory.CreatePageFor(model);
            return _view.AppNavigation.Push(page);
        }

        private void ViewOnPageLoadComplete(WebViewPageLoadEventArgs pageLoadEventArgs)
        {
            _singleSignOnMonitor.PageLoadComplete(pageLoadEventArgs);
        }

        private Task ViewOnSslError()
        {
            _logger.LogError("SSL Error encountered");

            var model = new ServiceDownErrorModel(_model.NavigationHandler, _model.FooterItem);
            var page = _pageFactory.CreatePageFor(model);
            return _view.AppNavigation.Push(page);
        }

        private async Task AddEventToCalendarRequested(AddEventToCalendarRequest request)
        {
            if (string.IsNullOrEmpty(request.Subject) ||
                request.StartTimeEpochInSeconds == null ||
                request.EndTimeEpochInSeconds == null ||
                request.StartTimeEpochInSeconds > request.EndTimeEpochInSeconds)
            {
                _logger.LogError("Passed calendar information is invalid, showing popup");
                await _calendar.ShowCalendarAlertWhenValidationFails().PreserveThreadContext();
            }
            else
            {
                var calendarPermission = await _calendar
                    .RequestPermission()
                    .PreserveThreadContext();

                if (calendarPermission)
                {
                    await _calendar.AddToCalendar(request).PreserveThreadContext();
                }
                else
                {
                    await _calendar.ShowCalendarPermissionDeniedAlert().PreserveThreadContext();
                }
            }
        }

        private async Task BackRequested()
        {
            _logger.LogInformation("Display back requested");
            await _view.NavigateBack().PreserveThreadContext();
        }
    }
}
