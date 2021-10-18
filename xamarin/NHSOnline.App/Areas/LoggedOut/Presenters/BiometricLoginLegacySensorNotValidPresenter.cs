using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginLegacySensorNotValidPresenter
    {
        private readonly ILogger<BiometricLoginLegacySensorNotValidPresenter> _logger;
        private readonly IBiometricLoginLegacySensorNotValidView _view;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly IUserPreferencesService _userPreferencesService;

        public BiometricLoginLegacySensorNotValidPresenter(
            ILogger<BiometricLoginLegacySensorNotValidPresenter> logger,
            IBiometricLoginLegacySensorNotValidView view,
            IBiometricAuthenticationService biometricAuthenticationService,
            IUserPreferencesService userPreferencesService)
        {
            _logger = logger;
            _view = view;
            _biometricAuthenticationService = biometricAuthenticationService;
            _userPreferencesService = userPreferencesService;

            _view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler(ViewOnBackToLoginRequested, (view, handler) => view.BackToLoginRequested = handler);
        }

        private async Task ViewOnAppearing()
        {
            await _biometricAuthenticationService
                .DeleteRegistration(_userPreferencesService.FidoUsername)
                .PreserveThreadContext();
        }

        private async Task ViewOnBackToLoginRequested()
        {
            _logger.LogInformation(nameof(ViewOnBackToLoginRequested));
            await _view.AppNavigation
                .PopToRoot()
                .PreserveThreadContext();
        }
    }
}