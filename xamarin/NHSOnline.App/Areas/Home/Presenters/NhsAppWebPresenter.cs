using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Session;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.LoggedOut;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Navigation;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class NhsAppWebPresenter
    {
        private bool _hasAlreadyAppeared;

        private readonly NhsAppWebModel _model;
        private readonly INhsAppWebView _view;
        private readonly INhsAppWebConfiguration _config;
        private readonly ILogger _logger;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly IPageFactory _pageFactory;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly RedirectorUrlFactory _redirectorUrlFactory;
        private readonly INhsAppNavigationHandler _navigationHandler;
        private readonly INotifications _notifications;
        private readonly ICalendar _calendar;
        private readonly ISettingsService _settingsService;
        private readonly IFileHandler _fileHandler;
        private readonly IAlertDialog _alertDialog;

        public NhsAppWebPresenter(
            NhsAppWebModel model,
            INhsAppWebView view,
            INhsAppWebConfiguration config,
            ILogger<NhsAppWebPresenter> logger,
            IBrowserOverlay browserOverlay,
            IPageFactory pageFactory,
            INotifications notifications,
            IBiometricAuthenticationService biometricAuthenticationService,
            RedirectorUrlFactory redirectorUrlFactory,
            ISettingsService settingsService,
            ICalendar calendar,
            IFileHandler fileHandler,
            IAlertDialog alertDialog)
        {
            _model = model;
            _view = view;
            _config = config;
            _logger = logger;
            _browserOverlay = browserOverlay;
            _pageFactory = pageFactory;
            _notifications = notifications;

            _biometricAuthenticationService = biometricAuthenticationService;
            _redirectorUrlFactory = redirectorUrlFactory;
            _settingsService = settingsService;
            _calendar = calendar;
            _fileHandler = fileHandler;
            _alertDialog = alertDialog;
            _navigationHandler = new NhsAppNavigationHandler(view);

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler<Uri>(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterHandler(HelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler<OpenWebIntegrationRequest>(OpenWebIntegrationRequested, (view, handler) => view.OpenWebIntegrationRequested = handler)
                .RegisterHandler<AddEventToCalendarRequest>(AddEventToCalendarRequested, (view, handler) => view.AddEventToCalendarRequested = handler)
                .RegisterHandler<DownloadRequest>(StartDownloadRequested, (view, handler) => view.StartDownloadRequested = handler)
                .RegisterHandler<StartNhsLoginUpliftRequest>(StartNhsLoginUpliftRequested, (view, handler) => view.StartNhsLoginUpliftRequested = handler)
                .RegisterHandler(ResetAndShowErrorRequested, (view, handler) => view.ResetAndShowErrorRequested = handler)
                .RegisterHandler(GetNotificationsStatusRequested, (view, handler) => view.GetNotificationsStatusRequested = handler)
                .RegisterHandler<string>(RequestPnsToken, (view, handler) => view.GetPnsTokenRequested = handler)
                .RegisterHandler<string>(FetchBiometricStatusRequested, (view, handler) => view.FetchBiometricStatusRequested = handler)
                .RegisterHandler<string>(UpdateBiometricRegistrationRequested, (view, handler) => view.UpdateBiometricRegistrationRequested = handler)
                .RegisterHandler<Uri>(OpenBrowserOverlayRequested, (view, handler) => view.OpenBrowserOverlayRequested = handler)
                .RegisterHandler<string>(SetMenuBarItemRequested, (view, handler) => view.SetMenuBarItemRequested = handler)
                .RegisterHandler(ClearMenuBarItemRequested, (view, handler) => view.ClearMenuBarItemRequested = handler)
                .RegisterHandler(OpenSettingsRequested, (view, handler) => view.OpenSettingsRequested = handler)
                .RegisterHandler(DisplayPageLeaveWarningRequested, (view, handler) => view.DisplayPageLeaveWarningRequested = handler)
                .RegisterHandler(OnSessionExpiringRequested, (view, handler) => view.OnSessionExpiringRequested = handler)
                .RegisterHandler(LogoutRequested, (view, handler) => view.LogoutRequested = handler)
                .RegisterHandler(SessionExpiredRequested, (view, handler) => view.SessionExpiredRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler<CreateOnDemandGpSessionRequest>(CreateOnDemandGpSessionRequested, (view, handler) => view.CreateOnDemandGpSessionRequested = handler)
                .RegisterHandler(_navigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler)
                .RegisterHandler(_navigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(_navigationHandler.AdviceRequested, (view, handler) => view.AdviceRequested = handler)
                .RegisterHandler(_navigationHandler.AppointmentsRequested, (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(_navigationHandler.PrescriptionsRequested, (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(_navigationHandler.YourHealthRequested, (view, handler) => view.YourHealthRequested = handler)
                .RegisterHandler(_navigationHandler.MessagesRequested, (view, handler) => view.MessagesRequested = handler)
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

        private Task DisplayPageLeaveWarningRequested()
        {
            _logger.LogInformation("Display page leave warning");

            _alertDialog.DisplayAlertDialog(
                "Leave this page?",
                "If you have entered any information, it will not be saved.",
                "Leave page",
                "Cancel",
                () => _view.SendLeavePage().PreserveThreadContext(),
                () => _view.SendStayOnPage().PreserveThreadContext()
            );

            return Task.CompletedTask;
        }

        private async Task OpenSettingsRequested()
        {
            _logger.LogInformation("Opening native settings");

            await _settingsService.OpenSettings().PreserveThreadContext();
        }

        private Task CreateOnDemandGpSessionRequested(CreateOnDemandGpSessionRequest request)
        {
            var popToRootNavigationHandler = new NhsAppPopToRootNavigationHandler(_navigationHandler, _view.AppNavigation);
            var model = new NhsLoginOnDemandGpSessionModel(request.AssertedLoginIdentity, request.RedirectTo, _view.SelectedNavigationFooterItem, popToRootNavigationHandler);
            var page = _pageFactory.CreatePageFor(model);

            return _view.AppNavigation.Push(page);
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
            var uri = new Uri(args.Url);
            if (!IsNhsAppWeb(uri))
            {
                args.Cancel = true;
                await _browserOverlay
                    .OpenBrowserOverlay(uri)
                    .PreserveThreadContext();
            }
        }

        private async Task OpenWebIntegrationRequested(OpenWebIntegrationRequest request)
        {
            _logger.LogInformation("Opening Web Integration - {Url}", request.Url);

            var popToRootNavigationHandler = new NhsAppPopToRootNavigationHandler(_navigationHandler, _view.AppNavigation);

            var model = new WebIntegrationModel(
                popToRootNavigationHandler,
                request.Url,
                _view.SelectedNavigationFooterItem,
                request.AdditionalDomains,
                request.HelpUrl);

            var page = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation
                .Push(page)
                .PreserveThreadContext();
        }

        private async Task StartDownloadRequested(DownloadRequest downloadRequest)
        {
            var storageWritePermissionCheck = await Permissions.CheckStatusAsync<Permissions.StorageWrite>().ResumeOnThreadPool();

            if (storageWritePermissionCheck == PermissionStatus.Granted)
            {
                await _fileHandler.StoreFileInDownloads(downloadRequest).PreserveThreadContext();
                await _fileHandler.HandleFile(downloadRequest).PreserveThreadContext();
            }
            else
            {
                var storageReadPermissionRequest = await Permissions.RequestAsync<Permissions.StorageWrite>().ResumeOnThreadPool();

                if (storageReadPermissionRequest == PermissionStatus.Granted)
                {
                   await _fileHandler.StoreFileInDownloads(downloadRequest).PreserveThreadContext();
                   await _fileHandler.HandleFile(downloadRequest).PreserveThreadContext();
                }
            }
        }

        private async Task AddEventToCalendarRequested(AddEventToCalendarRequest request)
        {
            _logger.LogInformation("Add event to calendar Requested - {Subject}", request.Subject);

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

        private async Task FetchBiometricStatusRequested(string accessToken)
        {
            var token = AccessToken.Parse(accessToken);
            var result = await _biometricAuthenticationService.FetchBiometricStatus(token.Subject).PreserveThreadContext();
            var biometricStatus = result.Accept(new BiometricResultToStatusVisitor());
            await _view.SendBiometricStatus(biometricStatus).PreserveThreadContext();
        }

        private Task SetMenuBarItemRequested(string menuItemIndex)
        {
            _logger.LogInformation($"Menu bar item change requested for {menuItemIndex}");

            var footerItem = GetFooterItemFromIndex(menuItemIndex);

            if (footerItem.HasValue)
            {
                _view.SelectedNavigationFooterItem = footerItem.Value;
            }
            else
            {
                _logger.LogError($"Menu bar item change requested for invalid index '{menuItemIndex}'");
            }

            return Task.CompletedTask;
        }

        private Task ClearMenuBarItemRequested()
        {
            _logger.LogInformation("Menu bar item clear requested");

            _view.SelectedNavigationFooterItem = NavigationFooterItem.None;
            return Task.CompletedTask;
        }

        private async Task UpdateBiometricRegistrationRequested(string accessToken)
        {
            var completion = new BiometricCompletion {Action = "Register"};
            var token = AccessToken.Parse(accessToken);

            try
            {
                var biometricStatusResult = await _biometricAuthenticationService.FetchBiometricStatus(token.Subject).PreserveThreadContext();
                switch (biometricStatusResult)
                {
                    case BiometricStatusResult.HardwarePresent { Registered: false, Usable: true }:
                        completion.Action = "Register";
                        var registerResult = await _biometricAuthenticationService.Register(token).PreserveThreadContext();
                        completion.Outcome = registerResult.Outcome.ToString();
                        completion.ErrorCode = registerResult.ErrorCode?.ToString() ?? string.Empty;
                        break;

                    case BiometricStatusResult.HardwarePresent { Registered: false, Usable: false }:
                        completion.Action = "Register";
                        completion.Outcome = BiometricOutcome.Failed.ToString();
                        completion.ErrorCode = BiometricErrorCode.CannotFindBiometrics.ToString();
                        break;

                    case BiometricStatusResult.HardwarePresent { Registered: true }:
                        completion.Action = "Deregister";
                        await _biometricAuthenticationService.DeleteRegistration(token).PreserveThreadContext();
                        completion.Outcome = BiometricOutcome.Success.ToString();
                        completion.ErrorCode = string.Empty;
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
                completion.ErrorCode = BiometricErrorCode.CannotChangeBiometrics.ToString();
            }

            await _view.SendBiometricCompletion(completion).PreserveThreadContext();
        }

        private Task BackRequested()
        {
            _logger.LogInformation("Display back requested");

            _alertDialog.DisplayAlertDialog(
                "Are you sure you want to log out?",
                "Log out",
                "Cancel",
                () => _view.Logout().PreserveThreadContext(),
                () => { }
            );

            return Task.CompletedTask;
        }

        private async Task LogoutRequested()
        {
            _logger.LogInformation("{Method}", nameof(LogoutRequested));

            var model = new LoggedOutHomeScreenModel();

            var page = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.PopToNewRoot(page).PreserveThreadContext();
        }

        private async Task SessionExpiredRequested()
        {
            _logger.LogInformation("{Method}", nameof(SessionExpiredRequested));

            var model = new LoggedOutHomeScreenModel(LoggedOutHomeScreenStates.SessionExpired);

            var page = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.PopToNewRoot(page).PreserveThreadContext();
        }

        private async Task HelpRequested()
        {
            await _view.GetContextualHelpLink().PreserveThreadContext();
        }

        private async Task OpenBrowserOverlayRequested(Uri overlayUri)
        {
            await _browserOverlay
                .OpenBrowserOverlay(overlayUri)
                .PreserveThreadContext();
        }

        private async Task DeeplinkRequested(Uri deeplinkUrl)
        {
            await _view.NavigateToRedirector(deeplinkUrl).PreserveThreadContext();
        }

        private bool IsNhsAppWeb(Uri uri)
        {
            return string.Equals(uri.Host, _config.Host, StringComparison.OrdinalIgnoreCase);
        }

        private Task DisplayNhsAppWeb()
        {
            if (_model.DeeplinkUrl != null)
            {
                _view.GoToUri(_redirectorUrlFactory.CreateUrlRedirect(_model.DeeplinkUrl));
                return Task.CompletedTask;
            }

            _view.GoToUri(_config.BaseAddress);
            return Task.CompletedTask;
        }

        private Task ViewOnNavigationFailed(Uri failingUrl)
        {
            var popToRootNavigationHandler = new NhsAppPopToRootNavigationHandler(_navigationHandler, _view.AppNavigation);
            void RetryAction() => _view.GoToUri(failingUrl);

            var model = new FullNavigationTryAgainNetworkErrorModel(popToRootNavigationHandler, _view.SelectedNavigationFooterItem, RetryAction);
            var page = _pageFactory.CreatePageFor(model);
            return _view.AppNavigation.Push(page);
        }

        private static NavigationFooterItem? GetFooterItemFromIndex(string footerItemIndex)
        {
            return footerItemIndex switch
            {
                "0" => NavigationFooterItem.Advice,
                "1" => NavigationFooterItem.Appointments,
                "2" => NavigationFooterItem.Prescriptions,
                "3" => NavigationFooterItem.YourHealth,
                "4" => NavigationFooterItem.Messages,
                _ => null,
            };
        }

        private sealed class BiometricResultToStatusVisitor : IBiometricStatusResultVisitor<BiometricStatus>
        {
            public BiometricStatus Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent)
                => BiometricStatus.None();

            public BiometricStatus Visit(BiometricStatusResult.FingerPrint fingerPrint)
                => BiometricStatus.FingerPrint(fingerPrint.Registered);

            public BiometricStatus Visit(BiometricStatusResult.TouchId touchId)
                => BiometricStatus.TouchId(touchId.Registered);

            public BiometricStatus Visit(BiometricStatusResult.FaceId faceId)
                => BiometricStatus.FaceId(faceId.Registered);
        }
    }
}
