using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class CloseSlimTryAgainNetworkErrorPresenter
    {
        private readonly ICloseSlimTryAgainNetworkErrorView _view;
        private readonly CloseSlimTryAgainNetworkErrorModel _model;
        private readonly IBrowser _browser;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;
        private readonly ILogger<CloseSlimTryAgainNetworkErrorPresenter> _logger;

        public CloseSlimTryAgainNetworkErrorPresenter(
            ICloseSlimTryAgainNetworkErrorView view,
            CloseSlimTryAgainNetworkErrorModel model,
            IBrowser browser,
            INhsExternalServicesConfiguration externalServicesConfiguration,
            ILogger<CloseSlimTryAgainNetworkErrorPresenter> logger)
        {
            _view = view;
            _model = model;
            _browser = browser;
            _externalServicesConfiguration = externalServicesConfiguration;
            _logger = logger;

            view.AppNavigation
                .RegisterHandler(CloseRequested, (view, handler) => view.CloseRequested = handler)
                .RegisterHandler(OneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(TryAgainRequested, (view, handler) => view.TryAgainRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler);
        }

        private async Task CloseRequested()
        {
            _logger.LogInformation("Close requested");

            await _view.AppNavigation.Pop().PreserveThreadContext();
            await _model.CloseAction.Invoke().PreserveThreadContext();
        }

        private async Task OneOneOneRequested()
        {
            _logger.LogInformation("OneOneOne requested");

            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private Task TryAgainRequested()
        {
            _logger.LogInformation("Try Again requested");

            return TryAgain();
        }

        private Task BackRequested()
        {
            _logger.LogInformation("Back requested");

            return TryAgain();
        }

        private async Task TryAgain()
        {
            await _view.AppNavigation.Pop().PreserveThreadContext();
            _model.RetryAction.Invoke();
        }
    }
}