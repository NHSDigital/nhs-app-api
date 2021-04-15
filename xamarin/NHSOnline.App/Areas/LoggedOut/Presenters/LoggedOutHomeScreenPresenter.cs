using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
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
        private readonly ILogger _logger;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly INhsLoginService _nhsLoginService;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly ILifecycle _lifecycle;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly IBackgroundExecutionService _backgroundExecutionService;

        public LoggedOutHomeScreenPresenter(
            ILoggedOutHomeScreenView view,
            ILogger<LoggedOutHomeScreenPresenter> logger,
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
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _biometricAuthenticationService = biometricAuthenticationService;
            _nhsLoginService = nhsLoginService;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _lifecycle = lifecycle;
            _browserOverlay = browserOverlay;
            _backgroundExecutionService = backgroundExecutionService;

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler(ViewOnLoginRequested, (view, handler) => view.LoginRequested = handler)
                .RegisterHandler(LoadCovidConditionsUrl, (view, handler) => view.NhsUkCovidConditionsServicePageRequested = handler)
                .RegisterHandler(LoadLoginHelpUrl, (view, handler) => view.NhsUkLoginHelpServicePageRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler(ResetAndShowErrorRequested, (view, handler) => view.ResetAndShowErrorRequested = handler);
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
            _logger.LogInformation("Login Requested");

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
            _logger.LogInformation("Back Requested");
            _lifecycle.CloseApplication();
            return Task.CompletedTask;
        }

        private async Task ShowGettingStartedPage()
        {
            var gettingStartedModel = new GettingStartedModel();
            var gettingStartedPage = _pageFactory.CreatePageFor(gettingStartedModel);
            await _view.AppNavigation.Push(gettingStartedPage).PreserveThreadContext();
        }

        private async Task ShowNhsLoginPage(string? fidoAuthResponse = null)
        {
            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes, fidoAuthResponse);
            var loginView = _pageFactory.CreatePageFor(loginModel);
            await _view.AppNavigation.Push(loginView).PreserveThreadContext();
        }

        private async Task ShowCouldNotLoginWithBiometrics()
        {
            var couldNotLoginModel = new BiometricLoginCouldNotLoginModel();
            var couldNotLoginView = _pageFactory.CreatePageFor(couldNotLoginModel);
            await _view.AppNavigation.Push(couldNotLoginView).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginFailed()
        {
            var result = await _biometricAuthenticationService.FetchBiometricStatus().PreserveThreadContext();
            await result.Accept(new BiometricLoginFailedStatusResultVisitor(this)).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginTouchIdFailed()
        {
            var model = new BiometricLoginTouchIdFailedModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.Push(view).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginFaceIdFailed()
        {
            var model = new BiometricLoginFaceIdFailedModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.Push(view).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginInvalidated()
        {
            _logger.LogInformation(nameof(ShowBiometricLoginInvalidated));
            var result = await _biometricAuthenticationService.FetchBiometricStatus().PreserveThreadContext();
            await result.Accept(new BiometricLoginInvalidatedStatusResultVisitor(this)).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginTouchIdInvalidated()
        {
            var model = new BiometricLoginTouchIdInvalidatedModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.Push(view).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginFaceIdInvalidated()
        {
            _logger.LogInformation(nameof(ShowBiometricLoginFaceIdInvalidated));
            var model = new BiometricLoginFaceIdInvalidatedModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.Push(view).PreserveThreadContext();
        }

        private async Task LoadCovidConditionsUrl()
        {
            _logger.LogInformation("Accessing covid conditions url");
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkCovidConditionsUrl)
                .PreserveThreadContext();
        }

        private async Task LoadLoginHelpUrl()
        {
            _logger.LogInformation("Accessing login help url");
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkLoginHelpUrl)
                .PreserveThreadContext();
        }

        private Task ResetAndShowErrorRequested()
        {
            // TODO ShowError
            _logger.LogInformation("Showing unexpected error");
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
                _presenter._logger.LogInformation("Biometric authorisation successful");
                await _presenter.ShowNhsLoginPage(authorised.FidoAuthResponse).PreserveThreadContext();
            }

            public async Task Visit(BiometricLoginResult.Unauthorised unauthorised)
            {
                _presenter._logger.LogInformation("Biometric authorisation unsuccessful");
                await _presenter.ShowCouldNotLoginWithBiometrics().PreserveThreadContext();
            }

            public async Task Visit(BiometricLoginResult.Failed failed)
            {
                _presenter._logger.LogInformation("Biometrics failed");
                await _presenter.ShowBiometricLoginFailed().PreserveThreadContext();
            }

            public Task Visit(BiometricLoginResult.Cancelled cancelled)
            {
                _presenter._logger.LogInformation("Biometrics cancelled");
                return Task.CompletedTask;
            }

            public Task Visit(BiometricLoginResult.NotRegistered notRegistered)
            {
                _presenter._logger.LogInformation("Biometrics not registered");
                return Task.CompletedTask;
            }

            public async Task Visit(BiometricLoginResult.Invalidated invalidated)
            {
                _presenter._logger.LogInformation("Biometrics invalidated");
                await _presenter.ShowBiometricLoginInvalidated().PreserveThreadContext();
            }
        }

        private sealed class BiometricLoginFailedStatusResultVisitor : IBiometricStatusResultVisitor<Task>
        {
            private readonly LoggedOutHomeScreenPresenter _presenter;

            public BiometricLoginFailedStatusResultVisitor(LoggedOutHomeScreenPresenter presenter)
            {
                _presenter = presenter;
            }

            public Task Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent)
            {
                _presenter._logger.LogError("Biometric login failed when hardware not present.");
                return Task.CompletedTask;
            }

            public Task Visit(BiometricStatusResult.FingerPrint fingerPrint)
                => throw new NotImplementedException();

            public async Task Visit(BiometricStatusResult.TouchId touchId)
                => await _presenter.ShowBiometricLoginTouchIdFailed().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.FaceId faceId)
                => await _presenter.ShowBiometricLoginFaceIdFailed().PreserveThreadContext();
        }

        private sealed class BiometricLoginInvalidatedStatusResultVisitor : IBiometricStatusResultVisitor<Task>
        {
            private readonly LoggedOutHomeScreenPresenter _presenter;

            public BiometricLoginInvalidatedStatusResultVisitor(LoggedOutHomeScreenPresenter presenter)
            {
                _presenter = presenter;
            }

            public Task Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent)
            {
                _presenter._logger.LogError("Biometric login invalidated when hardware not present.");
                return Task.CompletedTask;
            }

            public Task Visit(BiometricStatusResult.FingerPrint fingerPrint)
                => throw new NotImplementedException();

            public async Task Visit(BiometricStatusResult.TouchId touchId)
                => await _presenter.ShowBiometricLoginTouchIdInvalidated().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.FaceId faceId)
                => await _presenter.ShowBiometricLoginFaceIdInvalidated().PreserveThreadContext();
        }
    }
}
