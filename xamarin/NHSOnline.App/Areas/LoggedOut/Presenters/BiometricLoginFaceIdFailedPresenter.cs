using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginFaceIdFailedPresenter
    {
        private readonly ILogger<BiometricLoginFaceIdFailedPresenter> _logger;
        private readonly IBiometricLoginFaceIdFailedView _view;

        public BiometricLoginFaceIdFailedPresenter(
            ILogger<BiometricLoginFaceIdFailedPresenter> logger,
            IBiometricLoginFaceIdFailedView view)
        {
            _logger = logger;
            _view = view;

            _view.AppNavigation
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler);
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