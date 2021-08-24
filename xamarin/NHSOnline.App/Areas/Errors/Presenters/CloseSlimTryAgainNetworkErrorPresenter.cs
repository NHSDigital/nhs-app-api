using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class CloseSlimTryAgainNetworkErrorPresenter
    {
        private readonly ICloseSlimTryAgainNetworkErrorView _view;
        private readonly CloseSlimTryAgainNetworkErrorModel _model;
        private readonly ILogger<CloseSlimTryAgainNetworkErrorPresenter> _logger;

        public CloseSlimTryAgainNetworkErrorPresenter(
            ICloseSlimTryAgainNetworkErrorView view,
            CloseSlimTryAgainNetworkErrorModel model,
            ILogger<CloseSlimTryAgainNetworkErrorPresenter> logger)
        {
            _view = view;
            _model = model;
            _logger = logger;

            view.AppNavigation
                .RegisterHandler(TryAgainRequested, (view, handler) => view.TryAgainRequested = handler)
                .RegisterHandler(TryAgainRequested, (view, handler) => view.BackRequested = handler);
        }

        private async Task TryAgainRequested()
        {
            _logger.LogInformation("Try Again requested");

            await _view.AppNavigation.Pop().PreserveThreadContext();
            _model.RetryAction.Invoke();
        }
    }
}