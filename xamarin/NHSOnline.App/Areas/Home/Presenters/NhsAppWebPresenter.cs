using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Navigation;
using NHSOnline.App.Services;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class NhsAppWebPresenter
    {
        private readonly INhsAppWebView _view;
        private readonly NhsAppWebModel _model;
        private readonly INhsAppWebConfiguration _config;
        private readonly ILogger _logger;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly IPageFactory _pageFactory;
        private readonly INhsAppNavigationHandler _navigationHandler;
        private readonly INotifications _notifications;

        public NhsAppWebPresenter(
            INhsAppWebView view,
            NhsAppWebModel model,
            INhsAppWebConfiguration config,
            ILogger<NhsAppWebPresenter> logger,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IBrowserOverlay browserOverlay,
            IPageFactory pageFactory,
            INotifications notifications)
        {
            _view = view;
            _model = model;
            _config = config;
            _logger = logger;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _browserOverlay = browserOverlay;
            _pageFactory = pageFactory;
            _notifications = notifications;
            _navigationHandler = new NhsAppNavigationHandler(view);

            _view.Appearing = ViewOnAppearing;
            _view.Navigating = ViewOnNavigating;
            _view.Navigated = ViewOnNavigated;
            _view.HelpRequested = HelpRequested;
            _view.OpenWebIntegrationRequested = OpenWebIntegrationRequested;
            _view.StartNhsLoginUpliftRequested = StartNhsLoginUpliftRequested;
            _view.ResetAndShowErrorRequested = ResetAndShowErrorRequested;
            _view.GetNotificationsStatusRequested = GetNotificationsStatusRequested;
            _view.GetPnsTokenRequested = RequestPnsToken;

            _view.SettingsRequested = _navigationHandler.SettingsRequested;
            _view.HomeRequested = _navigationHandler.HomeRequested;
            _view.SymptomsRequested = _navigationHandler.SymptomsRequested;
            _view.AppointmentsRequested = _navigationHandler.AppointmentsRequested;
            _view.PrescriptionsRequested = _navigationHandler.PrescriptionsRequested;
            _view.RecordRequested = _navigationHandler.RecordRequested;
            _view.MoreRequested = _navigationHandler.MoreRequested;
        }

        private async Task ViewOnAppearing()
        {
            _view.Appearing = null;
            await DisplayNhsAppWeb().PreserveThreadContext();
        }

        private async Task ViewOnNavigating(WebNavigatingEventArgs args)
        {
            _logger.LogInformation("Navigating: {Uri}", args.Url);
            var uri = new Uri(args.Url);
            if (! IsNhsAppWeb(uri))
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

        private async Task OpenWebIntegrationRequested(OpenWebIntegrationRequest request)
        {
            _logger.LogInformation("Opening Web Integration - {Url}", request.Url);

            var popToRootNavigationHandler = new NhsAppPopToRootNavigationHandler(_navigationHandler, _view.Navigation);
            var model = new WebIntegrationModel(popToRootNavigationHandler, request.Url);

            var page = _pageFactory.CreatePageFor(model);
            await _view.Navigation
                .PushAsync(page)
                .PreserveThreadContext();
        }

        private async Task StartNhsLoginUpliftRequested(StartNhsLoginUpliftRequest request)
        {
            _logger.LogInformation("Starting Uplift - {Url}", request.Url);

            var model = new NhsLoginUpliftModel(request.Url);

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
                    authorisedResult);

                await _view.SendNotificationAuthorised(response).PreserveThreadContext();
            }
            else
            {
                await _view.SendNotificationUnauthorised().PreserveThreadContext();
            }
        }

        private async Task HelpRequested()
        {
            await _browserOverlay.OpenBrowserOverlay(
                _nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }

        private Task DisplayNhsAppWeb()
        {
            var homeUri = _config.BaseAddress;

            _view.GoToUri(homeUri);

            return Task.CompletedTask;
        }
    }
}
