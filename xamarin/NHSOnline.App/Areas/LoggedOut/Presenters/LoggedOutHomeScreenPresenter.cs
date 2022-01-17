using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Logging;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Services.ForcedUpdate;
using NHSOnline.App.Threading;
using Xamarin.Essentials;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class LoggedOutHomeScreenPresenter : IDisposable
    {
        private readonly ILoggedOutHomeScreenView _view;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly ILifecycle _lifecycle;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly IForcedUpdateCheckService _forcedUpdateCheckService;
        private readonly IDialogPresenter _dialogPresenterService;
        private readonly BiometricLoginErrorPageDispatcher _biometricLoginErrorPageDispatcher;
        private readonly ICookieService _cookieService;
        private readonly LoggedOutHomeScreenModel _model;

        private static readonly TimeSpan BiometricDelayOnAppearing = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan BiometricDelayOnTemporaryLockout = TimeSpan.FromSeconds(5);

        private Uri? _deeplinkUrl;
        private CancellationTokenSource _cancelBiometricLogin = new CancellationTokenSource();

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(LoggedOutHomeScreenPresenter));

        public LoggedOutHomeScreenPresenter(
            ILoggedOutHomeScreenView view,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            IBiometricAuthenticationService biometricAuthenticationService,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            ILifecycle lifecycle,
            IBrowserOverlay browserOverlay,
            IForcedUpdateCheckService forcedUpdateCheckService,
            IDialogPresenter dialogPresenterService,
            LoggedOutHomeScreenModel model,
            ICookieService cookieService)
        {
            _view = view;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _biometricAuthenticationService = biometricAuthenticationService;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _lifecycle = lifecycle;
            _browserOverlay = browserOverlay;
            _forcedUpdateCheckService = forcedUpdateCheckService;
            _dialogPresenterService = dialogPresenterService;
            _model = model;
            _cookieService = cookieService;

            _biometricLoginErrorPageDispatcher = new BiometricLoginErrorPageDispatcher(_view, _pageFactory,
                _biometricAuthenticationService, _userPreferencesService);

            _view.SetScreenState(_model.ScreenState);

            _view.VersionLabelText = $"Version {AppInfo.VersionString}";

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterPermanentHandler(ViewOnDisappearing, (view, handler) => view.Disappearing = handler)
                .RegisterHandler(ViewOnLoginRequested, (view, handler) => view.LoginRequested = handler)
                .RegisterHandler(LoadLoginHelpUrl, (view, handler) => view.NhsUkLoginHelpServicePageRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler(ResetAndShowErrorRequested, (view, handler) => view.ResetAndShowErrorRequested = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeeplinkRequested = handler);
        }

        private async Task ViewOnAppearing()
        {
            // Kick this Task off at this point so the update check can get underway for when needed.
            _forcedUpdateCheckService.Initiate();

            await _dialogPresenterService.DismissAll().PreserveThreadContext();
            _cancelBiometricLogin.Dispose();
            _cancelBiometricLogin = new CancellationTokenSource();
            await _cookieService.ClearSessionCookies().PreserveThreadContext();

            await ShowBiometricLoginAfterDelay(BiometricDelayOnAppearing).PreserveThreadContext();
        }

        private Task ViewOnDisappearing()
        {
            _cancelBiometricLogin.Cancel();
            return Task.CompletedTask;
        }

        private async Task ShowBiometricLoginAfterDelay(TimeSpan delay)
        {
            await Task.Delay(delay).PreserveThreadContext();

            if (!_cancelBiometricLogin.IsCancellationRequested)
            {
                await ActivateBiometricLogin().PreserveThreadContext();
            }
        }

        private async Task ActivateBiometricLogin()
        {
            var result = await _biometricAuthenticationService
                .Authenticate(_userPreferencesService.FidoUsername)
                .PreserveThreadContext();
            await result.Accept(new BiometricLoginResultVisitor(this)).PreserveThreadContext();
        }

        private async Task ViewOnLoginRequested()
        {
            Logger.LogInformation("Login Requested");

            _view.ResetScreenState();

            await BeginLoginJourney().PreserveThreadContext();
        }

        private async Task BeginLoginJourney(string? fidoAuthResponse = null)
        {
            var model = new BeginLoginModel(_deeplinkUrl, fidoAuthResponse);
            var page = _pageFactory.CreatePageFor(model);

            await _view.AppNavigation.Push(page).PreserveThreadContext();
        }

        private Task BackRequested()
        {
            Logger.LogInformation("Back Requested");
            _lifecycle.CloseApplication();
            return Task.CompletedTask;
        }

        private Task DeeplinkRequested(Uri deeplinkUrl)
        {
            _deeplinkUrl = deeplinkUrl;
            return Task.CompletedTask;
        }

        private async Task ShowCouldNotLoginWithBiometrics()
            => await _biometricLoginErrorPageDispatcher.ShowCouldNotLoginWithBiometrics().PreserveThreadContext();

        private async Task ShowBiometricLoginFailed()
        {
            await _biometricLoginErrorPageDispatcher.ShowBiometricLoginFailed().PreserveThreadContext();
            _cancelBiometricLogin.Cancel();
        }

        private async Task ShowBiometricLoginPermanentlyLockedOut()
            => await _biometricLoginErrorPageDispatcher.ShowBiometricLoginPermanentlyLockedOut().PreserveThreadContext();

        private async Task ShowBiometricLoginLegacySensorNotValid()
        {
            Logger.LogInformation("Showing legacy sensor not valid shutter screen");
            await _biometricLoginErrorPageDispatcher.ShowBiometricLoginLegacySensorNotValid().PreserveThreadContext();
        }

        private async Task LoadLoginHelpUrl()
        {
            Logger.LogInformation("Accessing login help url");
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkLoginHelpUrl)
                .PreserveThreadContext();
        }

        private Task ResetAndShowErrorRequested()
        {
            // TODO ShowError
            Logger.LogInformation("Showing unexpected error");
            return Task.CompletedTask;
        }

        private class BiometricLoginResultVisitor : IBiometricLoginResultVisitor<Task>
        {
            private readonly LoggedOutHomeScreenPresenter _presenter;

            public BiometricLoginResultVisitor(LoggedOutHomeScreenPresenter presenter)
            {
                _presenter = presenter;
            }

            public async Task Visit(BiometricLoginResult.Authorised authorised)
            {
                Logger.LogInformation("Biometric authorisation successful");
                await _presenter.BeginLoginJourney(authorised.FidoAuthResponse).PreserveThreadContext();
            }

            public async Task Visit(BiometricLoginResult.CouldNotLogin couldNotLogin)
            {
                Logger.LogInformation("Biometrics could not log in due to: {Reason}", couldNotLogin.Reason);
                await _presenter.ShowCouldNotLoginWithBiometrics().PreserveThreadContext();
            }

            public Task Visit(BiometricLoginResult.NoAction noAction)
            {
                Logger.LogInformation("No Biometric action due to: {Reason}", noAction.Reason);
                return Task.CompletedTask;
            }

            public async Task Visit(BiometricLoginResult.Failed failed)
            {
                Logger.LogInformation("Biometric login failed");
                await _presenter.ShowBiometricLoginFailed().PreserveThreadContext();
            }

            public async Task Visit(BiometricLoginResult.Lockout lockout)
            {
                if (lockout.Type == LockoutType.Permanent)
                {
                    Logger.LogInformation("Biometric login permanently locked out");
                    await _presenter.ShowBiometricLoginPermanentlyLockedOut().PreserveThreadContext();
                }
                else
                {
                    Logger.LogInformation("Biometric login temporarily locked out");
                    await _presenter.ShowBiometricLoginAfterDelay(BiometricDelayOnTemporaryLockout).PreserveThreadContext();
                }

            }

            public async Task Visit(BiometricLoginResult.LegacySensorNotValid legacySensorNotValid)
            {
                Logger.LogInformation("Biometric login legacy sensor not valid");
                await _presenter.ShowBiometricLoginLegacySensorNotValid().PreserveThreadContext();
            }
        }

        public void Dispose()
        {
            _cancelBiometricLogin.Dispose();
        }
    }
}
