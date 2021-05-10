using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Logging;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginErrorPageDispatcher
    {
        private readonly ILoggedOutHomeScreenView _view;
        private readonly IPageFactory _pageFactory;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly IUserPreferencesService _userPreferencesService;

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(BiometricLoginErrorPageDispatcher));

        public BiometricLoginErrorPageDispatcher(
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
            await _view.AppNavigation.PushAnimated(couldNotLoginView).PreserveThreadContext();
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
            await _view.AppNavigation.PushAnimated(view).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginFaceIdFailed()
        {
            var model = new BiometricLoginFaceIdFailedModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.PushAnimated(view).PreserveThreadContext();
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
            await _view.AppNavigation.PushAnimated(view).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginFaceIdPermanentlyLockedOut()
        {
            var model = new BiometricLoginFaceIdLockedOutModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.PushAnimated(view).PreserveThreadContext();
        }

        private sealed class BiometricLoginFailedStatusResultVisitor : IBiometricStatusResultVisitor<Task>
        {
            private readonly BiometricLoginErrorPageDispatcher _dispatcher;

            public BiometricLoginFailedStatusResultVisitor(BiometricLoginErrorPageDispatcher dispatcher)
            {
                _dispatcher = dispatcher;
            }

            public Task Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent)
            {
                Logger.LogError("Biometric login failed result when hardware not present");
                return Task.CompletedTask;
            }

            public async Task Visit(BiometricStatusResult.FingerPrint fingerPrint)
                => await _dispatcher.ShowBiometricLoginFingerprintFailed().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.TouchId touchId)
                => await _dispatcher.ShowBiometricLoginTouchIdFailed().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.FaceId faceId)
                => await _dispatcher.ShowBiometricLoginFaceIdFailed().PreserveThreadContext();
        }

        private sealed class BiometricLoginPermanentLockoutStatusResultVisitor : IBiometricStatusResultVisitor<Task>
        {
            private readonly BiometricLoginErrorPageDispatcher _dispatcher;

            public BiometricLoginPermanentLockoutStatusResultVisitor(BiometricLoginErrorPageDispatcher dispatcher)
            {
                _dispatcher = dispatcher;
            }

            public Task Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent)
            {
                Logger.LogError("Biometric login permanently locked out result when hardware not present");
                return Task.CompletedTask;
            }

            public async Task Visit(BiometricStatusResult.FingerPrint fingerPrint)
                => await _dispatcher.ShowBiometricLoginFingerprintPermanentlyLockedOut().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.TouchId touchId)
                => await _dispatcher.ShowBiometricLoginTouchIdPermanentlyLockedOut().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.FaceId faceId)
                => await _dispatcher.ShowBiometricLoginFaceIdPermanentlyLockedOut().PreserveThreadContext();
        }
    }
}