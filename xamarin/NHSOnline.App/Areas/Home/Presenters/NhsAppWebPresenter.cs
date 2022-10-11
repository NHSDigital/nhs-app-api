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
using NHSOnline.App.DependencyServices.Biometrics;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Dialogs;
using NHSOnline.App.Navigation;
using NHSOnline.App.Navigation.Handlers;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using BiometricStatus = NHSOnline.App.Controls.WebViews.Payloads.BiometricStatus;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class NhsAppWebPresenter
    {
        private const string HomePageRouteName = "index";

        private bool _hasAlreadyAppeared;

        private readonly NhsAppWebModel _model;
        private readonly INhsAppWebView _view;
        private readonly INhsAppWebConfiguration _config;
        private readonly ILogger _logger;
        private readonly IBrowser _browser;
        private readonly IPageFactory _pageFactory;
        private readonly INhsLoginService _nhsLoginService;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly RedirectorUrlFactory _redirectorUrlFactory;
        private readonly INhsAppNavigationHandler _navigationHandler;
        private readonly INotifications _notifications;
        private readonly ICalendar _calendar;
        private readonly ISettingsService _settingsService;
        private readonly IFileHandler _fileHandler;
        private readonly IDialogPresenter _dialogPresenter;
        private readonly FileDownloadService _fileDownloadService;
        private readonly IBiometrics _biometrics;

        public NhsAppWebPresenter(
            NhsAppWebModel model,
            INhsAppWebView view,
            INhsAppWebConfiguration config,
            ILogger<NhsAppWebPresenter> logger,
            IBrowser browser,
            IPageFactory pageFactory,
            INhsLoginService nhsLoginService,
            INotifications notifications,
            IBiometricAuthenticationService biometricAuthenticationService,
            RedirectorUrlFactory redirectorUrlFactory,
            ISettingsService settingsService,
            ICalendar calendar,
            IFileHandler fileHandler,
            IDialogPresenter dialogPresenter,
            FileDownloadService fileDownloadService,
            IBiometrics biometrics)
        {
            _model = model;
            _view = view;
            _config = config;
            _logger = logger;
            _browser = browser;
            _pageFactory = pageFactory;
            _nhsLoginService = nhsLoginService;
            _notifications = notifications;

            _biometricAuthenticationService = biometricAuthenticationService;
            _redirectorUrlFactory = redirectorUrlFactory;
            _settingsService = settingsService;
            _calendar = calendar;
            _fileHandler = fileHandler;
            _dialogPresenter = dialogPresenter;
            _fileDownloadService = fileDownloadService;
            _biometrics = biometrics;
            _navigationHandler = new NhsAppNavigationHandler(view);

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler<Uri>(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterHandler(HelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler<OpenWebIntegrationRequest>(OpenWebIntegrationRequested, (view, handler) => view.OpenWebIntegrationRequested = handler)
                .RegisterHandler<OpenPostWebIntegrationRequest>(OpenPostWebIntegrationRequested, (view, handler) => view.OpenPostWebIntegrationRequested = handler)
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
                .RegisterHandler(FetchNativeAppVersionRequested, (view, handler) => view.FetchNativeAppVersionRequested = handler)
                .RegisterHandler(ClearMenuBarItemRequested, (view, handler) => view.ClearMenuBarItemRequested = handler)
                .RegisterHandler(OpenSettingsRequested, (view, handler) => view.OpenSettingsRequested = handler)
                .RegisterHandler(DisplayPageLeaveWarningRequested, (view, handler) => view.DisplayPageLeaveWarningRequested = handler)
                .RegisterHandler(DisplayKeywordReplyPageLeaveWarningRequested, (view, handler) => view.DisplayKeywordReplyPageLeaveWarningRequested = handler)
                .RegisterHandler(OnSessionExpiringRequested, (view, handler) => view.OnSessionExpiringRequested = handler)
                .RegisterHandler(LogoutRequested, (view, handler) => view.LogoutRequested = handler)
                .RegisterHandler(SessionExpiredRequested, (view, handler) => view.SessionExpiredRequested = handler)
                .RegisterHandler<bool>(BackRequested, (view, handler) => view.BackRequested = handler)
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

        private async Task OnSessionExpiringRequested()
        {
            _logger.LogInformation("Display session expiring warning");

            await _dialogPresenter.DisplayAlertDialog(
                new SessionExpiry(
                    _view.SendSessionExtend,
                    _view.Logout)).PreserveThreadContext();
        }

        private async Task DisplayPageLeaveWarningRequested()
        {
            _logger.LogInformation("Display page leave warning");

            await _dialogPresenter.DisplayAlertDialog(
                new LeavePage(
                    _view.SendLeavePage,
                    _view.SendStayOnPage,
                    _view.SendStayOnPage)).PreserveThreadContext();
        }

        private async Task DisplayKeywordReplyPageLeaveWarningRequested()
        {
            _logger.LogInformation("Display keyword reply page leave warning");

            await _dialogPresenter.DisplayAlertDialog(
                new KeywordReplyLeavePage(
                    _view.SendLeavePage,
                    _view.SendStayOnPage,
                    _view.SendStayOnPage)).PreserveThreadContext();
        }

        private async Task OpenSettingsRequested()
        {
            _logger.LogInformation("Opening native settings");

            await _settingsService.OpenSettings().PreserveThreadContext();
        }

        private Task CreateOnDemandGpSessionRequested(CreateOnDemandGpSessionRequest request)
        {
            var model = new NhsLoginOnDemandGpSessionModel(request.AssertedLoginIdentity, request.RedirectTo, _view.SelectedNavigationFooterItem, GetNewPopToRootHandler());
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

        private void ViewOnNavigating(WebNavigatingEventArgs args)
        {
            try
            {
                var uri = new Uri(args.Url);
                if (!IsNhsAppWeb(uri))
                {
                    args.Cancel = true;

                    NhsAppResilience.ExecuteOnMainThread(() =>
                    {
                        _browser
                            .OpenBrowserOverlay(uri)
                            .PreserveThreadContext();
                    });
                }
            }
            catch (UriFormatException e)
            {
                var uriString = args.Url;
                var queryIndex = uriString.IndexOf("?", StringComparison.Ordinal);
                var urlSource = args.Source as UrlWebViewSource;
                var url = urlSource?.Url;
                _logger.LogError(e, "Failed to navigate to URI: {Uri} . Launched the failing URI from: {Url}",
                    queryIndex.Equals(-1) ? uriString : uriString.Remove(queryIndex), url);
            }
        }

        private NhsAppPopToRootNavigationHandler GetNewPopToRootHandler() => new NhsAppPopToRootNavigationHandler(_navigationHandler, _view.AppNavigation);

        private async Task OpenWebIntegrationRequested(OpenWebIntegrationRequest request)
        {
            _logger.LogInformation("Opening Web Integration - {Url}", request.Url);

            var model = new WebIntegrationModel(
                GetNewPopToRootHandler(),
                _view.SelectedNavigationFooterItem,
                WebIntegrationRequest.Create(request),
                request.AdditionalDomains,
                request.HelpUrl);

            var page = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation
                .Push(page)
                .PreserveThreadContext();
        }

        private async Task OpenPostWebIntegrationRequested(OpenPostWebIntegrationRequest request)
        {
            _logger.LogInformation("Opening Post Web Integration - {Url}", request.Url);

            var model = new WebIntegrationModel(
                GetNewPopToRootHandler(),
                _view.SelectedNavigationFooterItem,
                WebIntegrationRequest.Create(request),
                request.AdditionalDomains,
                request.HelpUrl);

            var page = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation
                .Push(page)
                .PreserveThreadContext();
        }

        private async Task StartDownloadRequested(DownloadRequest downloadRequest)
        {
            await _fileDownloadService.StartDownloadRequested(_dialogPresenter, downloadRequest, _fileHandler,
                _view.GetWebViewElement(),
                async () =>
                {
                    var model = new FullNavigationTryAgainFileDownloadErrorModel(GetNewPopToRootHandler(), _view.SelectedNavigationFooterItem);
                    var page = _pageFactory.CreatePageFor(model);
                    await _view.AppNavigation.Push(page).PreserveThreadContext();
                }).PreserveThreadContext();
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

        private async Task StartNhsLoginUpliftRequested(StartNhsLoginUpliftRequest request)
        {
            _logger.LogInformation("Starting Uplift");

            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var model = new NhsLoginUpliftModel(pkceCodes, request.AssertedLoginIdentity, GetNewPopToRootHandler());

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
            if (!_notifications.NotificationServiceAvailable())
            {
                await _view.SendNotificationsStatus(NotificationStatus.serviceError.ToString())
                    .PreserveThreadContext();

                return;
            }

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
            _logger.LogInformation("Menu bar item change requested for {menuItemIndex}", menuItemIndex);

            var footerItem = GetFooterItemFromIndex(menuItemIndex);

            if (footerItem.HasValue)
            {
                _view.SelectedNavigationFooterItem = footerItem.Value;
            }
            else
            {
                _logger.LogError("Menu bar item change requested for invalid index '{menuItemIndex}'", menuItemIndex);
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

                    // EnrolledAtDeviceLevel should only be used for ios Face ID
                    case BiometricStatusResult.HardwarePresent { Registered: false, Usable: false, EnrolledAtDeviceLevel: true }:
                        completion.Action = "Register";
                        completion.Outcome = BiometricOutcome.Failed.ToString();
                        completion.ErrorCode = BiometricErrorCode.CannotUseBiometrics.ToString();
                        break;

                    case BiometricStatusResult.HardwarePresent { Registered: false, Usable: false, EnrolledAtDeviceLevel: false }:
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

        private async Task BackRequested(bool shouldLogout)
        {
            _logger.LogInformation("Display back requested");

            var name = await _view.GetCurrentRouteName().PreserveThreadContext();

            if (string.Equals(name, HomePageRouteName, StringComparison.Ordinal))
            {
                if (shouldLogout)
                {
                    _logger.LogInformation("On home page and showing logout dialog");
                    await _dialogPresenter.DisplayAlertDialog(
                        new ConfirmLogout(_view.Logout)).PreserveThreadContext();
                }
                else
                {
                    _logger.LogInformation("On home page and not showing logout dialog");
                }
            }
            else
            {
                _logger.LogInformation("Page is not home, navigating back");
                await _view.NavigateBack().PreserveThreadContext();
            }
        }

        private async Task FetchNativeAppVersionRequested()
        {
            _logger.LogInformation("Fetching native app version");

            await _view.UpdateNativeVersion(AppInfo.VersionString).PreserveThreadContext();
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
            await _browser
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
            void RetryAction() => _view.GoToUri(failingUrl);
            Task CloseAction()
            {
                RetryAction();
                return Task.CompletedTask;
            }

            var model = new CloseSlimTryAgainNetworkErrorModel(CloseAction, RetryAction);
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

            public BiometricStatus Visit(BiometricStatusResult.LegacySensorNotValid legacySensorNotValid)
                => BiometricStatus.None();

            public BiometricStatus Visit(BiometricStatusResult.FingerPrintFaceOrIris fingerPrintFaceOrIris)
                => BiometricStatus.FingerPrintFaceOrIris(fingerPrintFaceOrIris.Registered);

            public BiometricStatus Visit(BiometricStatusResult.TouchId touchId)
                => BiometricStatus.TouchId(touchId.Registered);

            public BiometricStatus Visit(BiometricStatusResult.FaceId faceId)
                => BiometricStatus.FaceId(faceId.Registered);
        }
    }
}
