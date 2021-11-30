using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BiometricLoginCouldNotLoginPresenter
    {
        private readonly ILogger<BiometricLoginCouldNotLoginPresenter> _logger;
        private readonly IBiometricLoginCouldNotLoginView _view;

        public BiometricLoginCouldNotLoginPresenter(
            ILogger<BiometricLoginCouldNotLoginPresenter> logger,
            IBiometricLoginCouldNotLoginView view)
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