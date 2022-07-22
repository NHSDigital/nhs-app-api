using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Session;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.LoggedOut;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Areas.PreHome.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.DependencyServices.Notifications;
using NHSOnline.App.Dialogs;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Threading;
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
        private readonly IBrowser _browser;
        private readonly INotifications _notifications;
        private readonly ICookieService _cookieService;
        private readonly IDialogPresenter _dialogPresenter;
        private readonly IPreHomeLogoutMonitor _preHomeLogoutMonitor;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;

        private Uri? ResolveDeeplinkUrl => _deeplinkUrl ?? _model.DeeplinkUrl;

        public NhsAppPreHomeScreenWebPresenter(
            INhsAppPreHomeScreenWebView view,
            NhsAppPreHomeScreenWebModel model,
            INhsAppWebConfiguration config,
            ILogger<NhsAppPreHomeScreenWebPresenter> logger,
            IBrowser browser,
            IPageFactory pageFactory,
            INotifications notifications,
            IPreHomeLogoutMonitor preHomeLogoutMonitor,
            ICookieService cookieService,
            IDialogPresenter dialogPresenter,
            IBiometricAuthenticationService biometricAuthenticationService)
        {
            _view = view;
            _model = model;
            _config = config;
            _logger = logger;
            _browser = browser;
            _pageFactory = pageFactory;
            _notifications = notifications;
            _preHomeLogoutMonitor = preHomeLogoutMonitor;
            _cookieService = cookieService;
            _dialogPresenter = dialogPresenter;
            _biometricAuthenticationService = biometricAuthenticationService;

            _preHomeLogoutMonitor.Begin();

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler<Uri>(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterHandler<WebViewPageNavigationEventArgs>(ViewOnPageLoadComplete, (view, handler) => view.PageLoadComplete = handler)
                .RegisterHandler(GetNotificationsStatusRequested, (view, handler) => view.GetNotificationsStatusRequested = handler)
                .RegisterHandler<string>(FetchBiometricStatusRequested, (view, handler) => view.FetchBiometricStatusRequested = handler)
                .RegisterHandler<string>(UpdateBiometricRegistrationRequested, (view, handler) => view.UpdateBiometricRegistrationRequested = handler)
                .RegisterHandler(GoToLoggedInHomeRequested, (view, handler) => view.GoToLoggedInHomeRequested = handler)
                .RegisterHandler(LogoutRequested, (view, handler) => view.LogoutRequested = handler)
                .RegisterHandler<string>(RequestPnsToken, (view, handler) => view.GetPnsTokenRequested = handler)
                .RegisterHandler(ResetAndShowErrorRequested, (view, handler) => view.ResetAndShowErrorRequested = handler)
                .RegisterHandler(OnSessionExpiringRequested, (view, handler) => view.OnSessionExpiringRequested = handler)
                .RegisterHandler(SessionExpiredRequested, (view, handler) => view.SessionExpiredRequested = handler)
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

            _preHomeLogoutMonitor.Finish(await _view.GetCurrentWebViewUrl().PreserveThreadContext());

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

        private void ViewOnNavigating(WebNavigatingEventArgs args)
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

        private void ViewOnPageLoadComplete(WebViewPageNavigationEventArgs pageNavigationEventArgs)
        {
            _preHomeLogoutMonitor.PageLoadComplete(pageNavigationEventArgs);
        }

        private async Task GetNotificationsStatusRequested()
        {
            var notificationStatus = await _notifications.GetDeviceNotificationsStatus().PreserveThreadContext();
            await _view.SendNotificationsStatus(notificationStatus.ToString()).PreserveThreadContext();
        }

        private async Task FetchBiometricStatusRequested(string accessToken)
        {
            var token = AccessToken.Parse(accessToken);
            var result = await _biometricAuthenticationService.FetchBiometricStatus(token.Subject).PreserveThreadContext();
            var biometricStatus = result.Accept(new BiometricResultToStatusVisitor());
            await _view.SendBiometricStatus(biometricStatus).PreserveThreadContext();
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