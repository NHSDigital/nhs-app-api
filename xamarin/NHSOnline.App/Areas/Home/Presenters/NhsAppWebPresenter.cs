using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Navigation;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class NhsAppWebPresenter
    {
        private bool _hasAlreadyAppeared;

        private readonly INhsAppWebView _view;
        private readonly INhsAppWebConfiguration _config;
        private readonly ILogger _logger;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly IPageFactory _pageFactory;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly INhsAppNavigationHandler _navigationHandler;
        private readonly INotifications _notifications;

        public NhsAppWebPresenter(
            INhsAppWebView view,
            INhsAppWebConfiguration config,
            ILogger<NhsAppWebPresenter> logger,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IBrowserOverlay browserOverlay,
            IPageFactory pageFactory,
            INotifications notifications,
            IBiometricAuthenticationService biometricAuthenticationService)
        {
            _view = view;
            _config = config;
            _logger = logger;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _browserOverlay = browserOverlay;
            _pageFactory = pageFactory;
            _notifications = notifications;

            _biometricAuthenticationService = biometricAuthenticationService;
            _navigationHandler = new NhsAppNavigationHandler(view);

            _view.AppNavigation
                .RegisterHandler(
                    ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler<WebNavigatingEventArgs>(
                    ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler<WebNavigatedEventArgs>(
                    ViewOnNavigated, (view, handler) => view.Navigated = handler)
                .RegisterHandler(
                    HelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler<OpenWebIntegrationRequest>(
                    OpenWebIntegrationRequested, (view, handler) => view.OpenWebIntegrationRequested = handler)
                .RegisterHandler<StartNhsLoginUpliftRequest>(
                    StartNhsLoginUpliftRequested, (view, handler) => view.StartNhsLoginUpliftRequested = handler)
                .RegisterHandler(
                    ResetAndShowErrorRequested, (view, handler) => view.ResetAndShowErrorRequested = handler)
                .RegisterHandler(
                    GetNotificationsStatusRequested, (view, handler) => view.GetNotificationsStatusRequested = handler)
                .RegisterHandler<string>(
                    RequestPnsToken, (view, handler) => view.GetPnsTokenRequested = handler)
                .RegisterHandler(
                    FetchBiometricSpecRequested, (view, handler) => view.FetchBiometricSpecRequested = handler)
                .RegisterHandler<string>(
                    UpdateBiometricRegistrationRequested, (view, handler) => view.UpdateBiometricRegistrationRequested = handler)
                .RegisterHandler(
                    _navigationHandler.SettingsRequested, (view, handler) => view.SettingsRequested = handler)
                .RegisterHandler(
                    _navigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(
                    _navigationHandler.SymptomsRequested, (view, handler) => view.SymptomsRequested = handler)
                .RegisterHandler(
                    _navigationHandler.AppointmentsRequested, (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(
                    _navigationHandler.PrescriptionsRequested, (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(
                    _navigationHandler.RecordRequested, (view, handler) => view.RecordRequested = handler)
                .RegisterHandler(
                    _navigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler);
        }

        private async Task ViewOnAppearing()
        {
            if (_hasAlreadyAppeared)
            {
                return;
            }

            _hasAlreadyAppeared = true;

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

        private Task ViewOnNavigated(WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);
            return Task.CompletedTask;
        }

        private async Task OpenWebIntegrationRequested(OpenWebIntegrationRequest request)
        {
            _logger.LogInformation("Opening Web Integration - {Url}", request.Url);

            var popToRootNavigationHandler = new NhsAppPopToRootNavigationHandler(_navigationHandler, _view.AppNavigation);
            var model = new WebIntegrationModel(popToRootNavigationHandler, request.Url);

            var page = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation
                .Push(page)
                .PreserveThreadContext();
        }

        private async Task StartNhsLoginUpliftRequested(StartNhsLoginUpliftRequest request)
        {
            _logger.LogInformation("Starting Uplift - {Url}", request.Url);

            var model = new NhsLoginUpliftModel(request.Url);

            var page = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation
                .Push(page)
                .PreserveThreadContext();
        }

        private async Task ResetAndShowErrorRequested()
        {
            await DisplayNhsAppWeb().PreserveThreadContext();
            //TODO ShowError
            _logger.LogInformation("Showing unexpected error");
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

        private async Task FetchBiometricSpecRequested()
        {
            var biometricStatus = await _biometricAuthenticationService.FetchBiometricStatus().PreserveThreadContext();
            if (biometricStatus != null)
            {
                var biometricSpec = new BiometricSpec
                {
                    BiometricTypeReference = biometricStatus.BiometricTypeReference,
                    Enabled = biometricStatus.Enabled
                };
                await _view.SendBiometricSpec(biometricSpec).PreserveThreadContext();
            }
        }

        private async Task UpdateBiometricRegistrationRequested(string accessToken)
        {
            var completion = new BiometricCompletion {Action = "Register"};

            try
            {
                var biometricStatus = await _biometricAuthenticationService.FetchBiometricStatus().PreserveThreadContext();
                switch (biometricStatus)
                {
                    case { Enabled: false }:
                        completion.Action = "Register";
                        var registerResult = await _biometricAuthenticationService.Register().PreserveThreadContext();
                        completion.Outcome = registerResult.Outcome.ToString();
                        completion.ErrorCode = registerResult.ErrorCode?.ToString() ?? string.Empty;
                        break;
                    case { Enabled: true }:
                        completion.Action = "Deregister";
                        var deleteRegistrationResult = await _biometricAuthenticationService.DeleteRegistration().PreserveThreadContext();
                        completion.Outcome = deleteRegistrationResult.Outcome.ToString();
                        completion.ErrorCode = deleteRegistrationResult.ErrorCode?.ToString() ?? string.Empty;
                        break;
                    default:
                        completion.Outcome = BiometricOutcome.Failed.ToString();
                        completion.ErrorCode = BiometricErrorCode.CannotFindBiometrics.ToString();
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update biometric registration");
                completion.Outcome = BiometricOutcome.Failed.ToString();
            }

            await _view.SendBiometricCompletion(completion).PreserveThreadContext();
        }

        private async Task HelpRequested()
        {
            await _browserOverlay.OpenBrowserOverlay(
                _nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }

        private bool IsNhsAppWeb(Uri uri)
        {
            return string.Equals(uri.Host, _config.Host, StringComparison.OrdinalIgnoreCase);
        }

        private Task DisplayNhsAppWeb()
        {
            var homeUri = _config.BaseAddress;

            _view.GoToUri(homeUri);

            return Task.CompletedTask;
        }
    }
}
