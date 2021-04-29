using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Logging;
using NHSOnline.App.Services.FIDO;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginErrorPageDispatcher
    {
        private readonly ILoggedOutHomeScreenView _view;
        private readonly IPageFactory _pageFactory;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;

        private static ILogger Logger => NhsAppLogging.CreateLogger(typeof(BiometricLoginErrorPageDispatcher));

        public BiometricLoginErrorPageDispatcher(
            ILoggedOutHomeScreenView view,
            IPageFactory pageFactory,
            IBiometricAuthenticationService biometricAuthenticationService)
        {
            _view = view;
            _pageFactory = pageFactory;
            _biometricAuthenticationService = biometricAuthenticationService;
        }

        public async Task ShowCouldNotLoginWithBiometrics()
        {
            var couldNotLoginModel = new BiometricLoginCouldNotLoginModel();
            var couldNotLoginView = _pageFactory.CreatePageFor(couldNotLoginModel);
            await _view.AppNavigation.PushAnimated(couldNotLoginView).PreserveThreadContext();
        }

        public async Task ShowBiometricLoginFailed()
        {
            var result = await _biometricAuthenticationService.FetchBiometricStatus().PreserveThreadContext();
            await result.Accept(new BiometricLoginFailedStatusResultVisitor(this)).PreserveThreadContext();
        }

        public async Task ShowBiometricLoginInvalidated()
        {
            var result = await _biometricAuthenticationService.FetchBiometricStatus().PreserveThreadContext();
            await result.Accept(new BiometricLoginInvalidatedStatusResultVisitor(this)).PreserveThreadContext();
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

        private async Task ShowBiometricLoginTouchIdInvalidated()
        {
            var model = new BiometricLoginTouchIdInvalidatedModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.PushAnimated(view).PreserveThreadContext();
        }

        private async Task ShowBiometricLoginFaceIdInvalidated()
        {
            var model = new BiometricLoginFaceIdInvalidatedModel();
            var view = _pageFactory.CreatePageFor(model);
            await _view.AppNavigation.PushAnimated(view).PreserveThreadContext();
        }

        private sealed class BiometricLoginInvalidatedStatusResultVisitor : IBiometricStatusResultVisitor<Task>
        {
            private readonly BiometricLoginErrorPageDispatcher _handler;

            public BiometricLoginInvalidatedStatusResultVisitor(BiometricLoginErrorPageDispatcher handler)
            {
                _handler = handler;
            }

            public Task Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent)
            {
                Logger.LogError("Biometric login invalidated when hardware not present");
                return Task.CompletedTask;
            }

            public Task Visit(BiometricStatusResult.FingerPrint fingerPrint)
                => throw new NotImplementedException();

            public async Task Visit(BiometricStatusResult.TouchId touchId)
                => await _handler.ShowBiometricLoginTouchIdInvalidated().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.FaceId faceId)
                => await _handler.ShowBiometricLoginFaceIdInvalidated().PreserveThreadContext();
        }

        private sealed class BiometricLoginFailedStatusResultVisitor : IBiometricStatusResultVisitor<Task>
        {
            private readonly BiometricLoginErrorPageDispatcher _handler;

            public BiometricLoginFailedStatusResultVisitor(BiometricLoginErrorPageDispatcher handler)
            {
                _handler = handler;
            }

            public Task Visit(BiometricStatusResult.HardwareNotPresent hardwareNotPresent)
            {
                Logger.LogError("Biometric login failed when hardware not present");
                return Task.CompletedTask;
            }

            public Task Visit(BiometricStatusResult.FingerPrint fingerPrint)
                => throw new NotImplementedException();

            public async Task Visit(BiometricStatusResult.TouchId touchId)
                => await _handler.ShowBiometricLoginTouchIdFailed().PreserveThreadContext();

            public async Task Visit(BiometricStatusResult.FaceId faceId)
                => await _handler.ShowBiometricLoginFaceIdFailed().PreserveThreadContext();
        }
    }
}