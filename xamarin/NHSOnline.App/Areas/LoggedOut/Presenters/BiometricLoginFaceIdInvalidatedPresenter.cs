using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Services.FIDO;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginFaceIdInvalidatedPresenter
    {
        private readonly ILogger<BiometricLoginFaceIdInvalidatedPresenter> _logger;
        private readonly IBiometricLoginFaceIdInvalidatedView _view;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;

        public BiometricLoginFaceIdInvalidatedPresenter(
            ILogger<BiometricLoginFaceIdInvalidatedPresenter> logger,
            IBiometricLoginFaceIdInvalidatedView view,
            IBiometricAuthenticationService biometricAuthenticationService)
        {
            _logger = logger;
            _view = view;
            _biometricAuthenticationService = biometricAuthenticationService;

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler);
        }

        private async Task ViewOnAppearing()
        {
            await _biometricAuthenticationService
                .DeleteAuthKey()
                .PreserveThreadContext();
        }

        private async Task ViewOnBackHomeRequested()
        {
            _logger.LogInformation(nameof(ViewOnBackHomeRequested));
            await _view.AppNavigation
                .PopToRoot()
                .PreserveThreadContext();
        }
    }
}