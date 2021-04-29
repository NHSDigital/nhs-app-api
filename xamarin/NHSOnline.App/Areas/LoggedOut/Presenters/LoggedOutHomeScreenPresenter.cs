using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Logging;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Threading;
using Xamarin.Essentials;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class LoggedOutHomeScreenPresenter
    {
        private readonly ILoggedOutHomeScreenView _view;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly INhsLoginService _nhsLoginService;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly ILifecycle _lifecycle;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly IBackgroundExecutionService _backgroundExecutionService;
        private readonly BiometricLoginErrorPageDispatcher _biometricLoginErrorPageDispatcher;

        private Uri? _deeplinkUrl;
        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(LoggedOutHomeScreenPresenter));


        public LoggedOutHomeScreenPresenter(
            ILoggedOutHomeScreenView view,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            IBiometricAuthenticationService biometricAuthenticationService,
            INhsLoginService nhsLoginService,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            ILifecycle lifecycle,
            IBrowserOverlay browserOverlay,
            IBackgroundExecutionService backgroundExecutionService)
        {
            _view = view;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _biometricAuthenticationService = biometricAuthenticationService;
            _nhsLoginService = nhsLoginService;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _lifecycle = lifecycle;
            _browserOverlay = browserOverlay;
            _backgroundExecutionService = backgroundExecutionService;

            _biometricLoginErrorPageDispatcher = new BiometricLoginErrorPageDispatcher(_view, _pageFactory, _biometricAuthenticationService);

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler(ViewOnLoginRequested, (view, handler) => view.LoginRequested = handler)
                .RegisterHandler(LoadCovidConditionsUrl,
                    (view, handler) => view.NhsUkCovidConditionsServicePageRequested = handler)
                .RegisterHandler(LoadLoginHelpUrl, (view, handler) => view.NhsUkLoginHelpServicePageRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler(ResetAndShowErrorRequested,
                    (view, handler) => view.ResetAndShowErrorRequested = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeeplinkRequested = handler);
        }

        private async Task ViewOnAppearing()
        {
            // NHSO-14252 will address this workaround
            await _backgroundExecutionService.Run(async () =>
            {
                await Task.Delay(100).ResumeOnThreadPool();

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var result = await _biometricAuthenticationService.Authenticate().PreserveThreadContext();
                    await result.Accept(new BiometricLoginResultVisitor(this)).PreserveThreadContext();
                });
            }).ResumeOnThreadPool();
        }

        private async Task ViewOnLoginRequested()
        {
            Logger.LogInformation("Login Requested");

            if (_userPreferencesService.ShowGettingStarted)
            {
                await ShowGettingStartedPage().PreserveThreadContext();
            }
            else
            {
                await ShowNhsLoginPage().PreserveThreadContext();
            }
        }

        private Task BackRequested()
        {
            Logger.LogInformation("Back Requested");
            _lifecycle.CloseApplication();
            return Task.CompletedTask;
        }

        private async Task ShowGettingStartedPage()
        {
            var gettingStartedModel = new GettingStartedModel(_deeplinkUrl);
            var gettingStartedPage = _pageFactory.CreatePageFor(gettingStartedModel);
            await _view.AppNavigation.PushAnimated(gettingStartedPage).PreserveThreadContext();
        }

        private Task DeeplinkRequested(Uri deeplinkUrl)
        {
            _deeplinkUrl = deeplinkUrl;
            return Task.CompletedTask;
        }

        private async Task ShowNhsLoginPage(string? fidoAuthResponse = null)
        {
            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes, fidoAuthResponse, _deeplinkUrl);
            var loginView = _pageFactory.CreatePageFor(loginModel);
            await _view.AppNavigation.PushAnimated(loginView).PreserveThreadContext();
        }

        private async Task ShowCouldNotLoginWithBiometrics()
            => await _biometricLoginErrorPageDispatcher.ShowCouldNotLoginWithBiometrics().PreserveThreadContext();

        private async Task ShowBiometricLoginFailed()
            => await _biometricLoginErrorPageDispatcher.ShowBiometricLoginFailed().PreserveThreadContext();

        private async Task ShowBiometricLoginInvalidated()
            => await _biometricLoginErrorPageDispatcher.ShowBiometricLoginInvalidated().PreserveThreadContext();

        private async Task LoadCovidConditionsUrl()
        {
            Logger.LogInformation("Accessing covid conditions url");
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkCovidConditionsUrl)
                .PreserveThreadContext();
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
                await _presenter.ShowNhsLoginPage(authorised.FidoAuthResponse).PreserveThreadContext();
            }

            public async Task Visit(BiometricLoginResult.Unauthorised unauthorised)
            {
                Logger.LogInformation("Biometric authorisation unsuccessful");
                await _presenter.ShowCouldNotLoginWithBiometrics().PreserveThreadContext();
            }

            public async Task Visit(BiometricLoginResult.Failed failed)
            {
                Logger.LogInformation("Biometrics failed");
                await _presenter.ShowBiometricLoginFailed().PreserveThreadContext();
            }

            public Task Visit(BiometricLoginResult.UserCancelled userCancelled)
            {
                Logger.LogInformation("Biometrics cancelled by the user");
                return Task.CompletedTask;
            }

            public async Task Visit(BiometricLoginResult.SystemCancelled systemCancelled)
            {
                Logger.LogInformation("Biometrics cancelled by the system");
                await _presenter.ShowCouldNotLoginWithBiometrics().PreserveThreadContext();
            }

            public Task Visit(BiometricLoginResult.NotRegistered notRegistered)
            {
                Logger.LogInformation("Biometrics not registered");
                return Task.CompletedTask;
            }

            public async Task Visit(BiometricLoginResult.Invalidated invalidated)
            {
                Logger.LogInformation("Biometrics invalidated");
                await _presenter.ShowBiometricLoginInvalidated().PreserveThreadContext();
            }
        }
    }
}
