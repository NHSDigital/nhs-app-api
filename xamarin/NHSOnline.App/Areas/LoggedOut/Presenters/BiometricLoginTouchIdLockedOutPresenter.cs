using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginTouchIdLockedOutPresenter
    {
        private readonly ILogger<BiometricLoginTouchIdLockedOutPresenter> _logger;
        private readonly IBiometricLoginTouchIdLockedOutView _view;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private readonly IUserPreferencesService _userPreferencesService;

        public BiometricLoginTouchIdLockedOutPresenter(
            ILogger<BiometricLoginTouchIdLockedOutPresenter> logger,
            IBiometricLoginTouchIdLockedOutView view,
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
            _logger.LogInformation(nameof(ViewOnBackHomeRequested));
            await _view.AppNavigation
                .PopToRoot()
                .PreserveThreadContext();
        }
    }
}