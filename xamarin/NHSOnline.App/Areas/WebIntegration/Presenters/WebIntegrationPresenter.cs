using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal sealed class WebIntegrationPresenter
    {
        private bool _hasAlreadyAppeared;

        private readonly IWebIntegrationView _view;
        private readonly WebIntegrationModel _model;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly ILogger _logger;
        private readonly WebIntegrationUriDestination _uriDestination;
        private readonly ICalendar _calendar;
        private readonly IFileHandler _fileHandler;

        public WebIntegrationPresenter(
            IWebIntegrationView view,
            WebIntegrationModel model,
            INhsLoginConfiguration nhsLoginConfiguration,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IBrowserOverlay browserOverlay,
            ILogger<WebIntegrationPresenter> logger,
            ICalendar calendar,
            IFileHandler fileHandler)
        {
            _view = view;
            _model = model;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _browserOverlay = browserOverlay;
            _logger = logger;
            _calendar = calendar;
            _fileHandler = fileHandler;

            _uriDestination = new WebIntegrationUriDestination(nhsLoginConfiguration, model.Url, model.AdditionalDomains);

            _view.SetNavigationFooterItem(model.FooterItem);

            _view.AppNavigation
                .RegisterHandler(
                    ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler(
                    HelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler<WebNavigatingEventArgs>(
                    ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler(
                    model.NavigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.AdviceRequested, (view, handler) => view.AdviceRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.AppointmentsRequested,
                    (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.PrescriptionsRequested,
                    (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.YourHealthRequested, (view, handler) => view.YourHealthRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.MessagesRequested, (view, handler) => view.MessagesRequested = handler)
                .RegisterHandler<string>(
                    GoToNhsAppPageRequested, (view, handler) => view.GoToNhsAppPageRequested = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested,
                    (view, handler) => view.DeepLinkRequested = handler)
                .RegisterHandler<AddEventToCalendarRequest>(AddEventToCalendarRequested,
                    (view, handler) => view.AddEventToCalendarRequested = handler)
                .RegisterHandler<DownloadRequest>(StartDownloadRequested,
                (view, handler) => view.StartDownloadRequested = handler);

        }

        private async Task GoToNhsAppPageRequested(string page)
        {
            _logger.LogInformation("Going to NHS App Page - {page}", page);

            await _model.NavigationHandler.GoToNhsAppPageRequested(page).PreserveThreadContext();
        }

        private async Task StartDownloadRequested(DownloadRequest downloadRequest)
        {
            var storagePermissionCheck = await Permissions.CheckStatusAsync<Permissions.StorageWrite>().ResumeOnThreadPool();

            if (storagePermissionCheck == PermissionStatus.Granted)
            {
                await _fileHandler.StoreFileInDownloads(downloadRequest).PreserveThreadContext();
                await _fileHandler.HandleFile(downloadRequest).PreserveThreadContext();
            }
            else
            {
                var storagePermissionRequest = await Permissions.RequestAsync<Permissions.StorageWrite>().ResumeOnThreadPool();

                if (storagePermissionRequest == PermissionStatus.Granted)
                {
                    await _fileHandler.StoreFileInDownloads(downloadRequest).PreserveThreadContext();
                    await _fileHandler.HandleFile(downloadRequest).PreserveThreadContext();
                }
            }
        }

        private async Task DeeplinkRequested(Uri deepLinkUrl)
        {
            _logger.LogInformation("Redirecting to deeplink - {deeplink}", deepLinkUrl);

            await _model.NavigationHandler.RedirectToDeepLinkRequested(deepLinkUrl).PreserveThreadContext();
        }

        private async Task HelpRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }

        private Task ViewOnAppearing()
        {
            if (_hasAlreadyAppeared)
            {
                return Task.CompletedTask;
            }

            _hasAlreadyAppeared = true;

            _view.GoToUri(_model.Url);

            return Task.CompletedTask;
        }

        private async Task ViewOnNavigating(WebNavigatingEventArgs webNavigatingEventArgs)
        {
            var url = new Uri(webNavigatingEventArgs.Url);
            if (_uriDestination.ShouldOpenInBrowserOverlay(url))
            {
                await OpenInBrowserOverlay(webNavigatingEventArgs, url).PreserveThreadContext();
            }
        }

        private async Task OpenInBrowserOverlay(WebNavigatingEventArgs webNavigatingEventArgs, Uri url)
        {
            webNavigatingEventArgs.Cancel = true;
            await _browserOverlay.OpenBrowserOverlay(url).PreserveThreadContext();
        }

        private async Task AddEventToCalendarRequested(AddEventToCalendarRequest request)
        {
            if (string.IsNullOrEmpty(request.Subject) ||
                request.StartTimeEpochInSeconds == null ||
                request.EndTimeEpochInSeconds == null ||
                request.StartTimeEpochInSeconds > request.EndTimeEpochInSeconds)
            {
                _logger.LogError("Passed calendar information is invalid, showing popup");
                _calendar.ShowCalendarAlertWhenValidationFails();
            }
            else
            {
                var calendarPermission = await _calendar
                    .RequestPermission()
                    .PreserveThreadContext();

                if (calendarPermission)
                {
                    _calendar.AddToCalendar(request);
                }
                else
                {
                    _calendar.ShowCalendarPermissionDeniedAlert();
                }
            }
        }
    }
}
