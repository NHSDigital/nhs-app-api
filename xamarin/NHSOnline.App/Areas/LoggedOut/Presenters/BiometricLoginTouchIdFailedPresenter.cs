using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginTouchIdFailedPresenter
    {
        private readonly ILogger<BiometricLoginTouchIdFailedPresenter> _logger;
        private readonly IBiometricLoginTouchIdFailedView _view;

        public BiometricLoginTouchIdFailedPresenter(
            ILogger<BiometricLoginTouchIdFailedPresenter> logger,
            IBiometricLoginTouchIdFailedView view)
        {
            _logger = logger;
            _view = view;

            _view.AppNavigation
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler);
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