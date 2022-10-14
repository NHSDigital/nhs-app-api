using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Logging;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Services.UserPreferences;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginErrorPagePresenter
    {
        private readonly ILoggedOutHomeScreenView _view;
        private readonly IPageFactory _pageFactory;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly IUserPreferencesService _userPreferencesService;

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(BiometricLoginErrorPagePresenter));

        public BiometricLoginErrorPagePresenter(
            ILoggedOutHomeScreenView view,
            IPageFactory pageFactory,
            IBiometricAuthenticationService biometricAuthenticationService,
            IUserPreferencesService userPreferencesService)
        {
            _view = view;
            _pageFactory = pageFactory;
            _biometricAuthenticationService = biometricAuthenticationService;
            _userPreferencesService = userPreferencesService;
        }

        public async Task ShowCouldNotLoginWithBiometrics()
        {
            var couldNotLoginModel = new BiometricLoginCouldNotLoginModel();
            var couldNotLoginView = _pageFactory.CreatePageFor(couldNotLoginModel);
            await _view.AppNavigation.Push(couldNotLoginView).PreserveThreadContext();
        }

        public async Task ShowBiometricLoginFailed()
        {
            var result = await _biometricAuthenticationService
                .FetchBiometricStatus(_userPreferencesService.FidoUsername)
                .PreserveThreadContext();
            await result.Accept(new BiometricLoginFailedStatusResultVisitor(this)).PreserveThreadContext();
        }

        public async Task ShowBiometricLoginPermanentlyLockedOut()
        {
            var result = await _biometricAuthenticationService
                .FetchBiometricStatus(_userPreferencesService.FidoUsername)
                .PreserveThreadContext();
            await result.Accept(new BiometricLoginPermanentLockoutStatusResultVisitor(this)).PreserveThreadContext();
        }

        public async Task ShowBiometricLoginLegacySensorNotValid()
        {
            var model = new BiometricLoginLegacySensorNotValidModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.Push(view).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginFingerprintFailed()
        {
            var model = new BiometricLoginFingerprintFailedModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.Push(view).PreserveThreadContext();
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

        private async Task ShowBiometricLoginFingerprintPermanentlyLockedOut()
        {
            var model = new BiometricLoginFingerprintLockedOutModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.Push(view).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginTouchIdPermanentlyLockedOut()
        {
            var model = new BiometricLoginTouchIdLockedOutModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.Push(view).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginFaceIdPermanentlyLockedOut()
        {
            var model = new BiometricLoginFaceIdLockedOutModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.Push(view).PreserveThreadContext();
        }

        private sealed class BiometricLoginFailedStatusResultVisitor : IBiometricStatusResultVisitor<Task>
        {
            private readonly BiometricLoginErrorPagePresenter _presenter;

            public BiometricLoginFailedStatusResultVisitor(BiometricLoginErrorPagePresenter presenter)
            {
                _presenter = presenter;
            }

            public Task Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent)
            {
                Logger.LogError("Biometric login failed result when hardware not present");
                return Task.CompletedTask;
            }

            public Task Visit(BiometricStatusResult.LegacySensorNotValid legacySensorNotValid)
            {
                Logger.LogError("Biometric login failed result when legacy sensor not valid");
                return Task.CompletedTask;
            }

            public async Task Visit(BiometricStatusResult.FingerPrintFaceOrIris fingerPrintFaceOrIris)
                => await _presenter.ShowBiometricLoginFingerprintFailed().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.TouchId touchId)
                => await _presenter.ShowBiometricLoginTouchIdFailed().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.FaceId faceId)
                => await _presenter.ShowBiometricLoginFaceIdFailed().PreserveThreadContext();
        }

        private sealed class BiometricLoginPermanentLockoutStatusResultVisitor : IBiometricStatusResultVisitor<Task>
        {
            private readonly BiometricLoginErrorPagePresenter _presenter;

            public BiometricLoginPermanentLockoutStatusResultVisitor(BiometricLoginErrorPagePresenter presenter)
            {
                _presenter = presenter;
            }

            public Task Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent)
            {
                Logger.LogError("Biometric login permanently locked out result when hardware not present");
                return Task.CompletedTask;
            }

            public Task Visit(BiometricStatusResult.LegacySensorNotValid legacySensorNotValid)
            {
                Logger.LogError("Biometric login permanently locked out result when legacy sensor not valid");
                return Task.CompletedTask;
            }

            public async Task Visit(BiometricStatusResult.FingerPrintFaceOrIris fingerPrintFaceOrIris)
                => await _presenter.ShowBiometricLoginFingerprintPermanentlyLockedOut().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.TouchId touchId)
                => await _presenter.ShowBiometricLoginTouchIdPermanentlyLockedOut().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.FaceId faceId)
                => await _presenter.ShowBiometricLoginFaceIdPermanentlyLockedOut().PreserveThreadContext();
        }
    }
}