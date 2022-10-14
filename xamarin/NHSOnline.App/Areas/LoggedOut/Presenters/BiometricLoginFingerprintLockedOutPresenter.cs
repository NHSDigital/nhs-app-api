using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Services.UserPreferences;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginFingerprintLockedOutPresenter
    {
        private readonly ILogger<BiometricLoginFingerprintLockedOutPresenter> _logger;
        private readonly IBiometricLoginFingerprintFaceIrisLockedOutView _view;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly IUserPreferencesService _userPreferencesService;

        public BiometricLoginFingerprintLockedOutPresenter(
            ILogger<BiometricLoginFingerprintLockedOutPresenter> logger,
            IBiometricLoginFingerprintFaceIrisLockedOutView view,
            IBiometricAuthenticationService biometricAuthenticationService,
            IUserPreferencesService userPreferencesService)
        {
            _logger = logger;
            _view = view;
            _biometricAuthenticationService = biometricAuthenticationService;
            _userPreferencesService = userPreferencesService;

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler);
        }

        private async Task ViewOnAppearing()
        {
            await _biometricAuthenticationService
                .DeleteRegistration(_userPreferencesService.FidoUsername)
                .PreserveThreadContext();
        }

        private async Task ViewOnBackHomeRequested()
        {
            _logger.LogInformation("{Method}", nameof(ViewOnBackHomeRequested));
            await _view.AppNavigation
                .PopToRoot()
                .PreserveThreadContext();
        }
    }
}