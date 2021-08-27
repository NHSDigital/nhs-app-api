using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class PreHomeTryAgainNetworkErrorPresenter
    {
        private readonly IPreHomeTryAgainNetworkErrorView _view;
        private readonly PreHomeTryAgainNetworkErrorModel _model;
        private readonly ILogger<PreHomeTryAgainNetworkErrorPresenter> _logger;

        public PreHomeTryAgainNetworkErrorPresenter(
            IPreHomeTryAgainNetworkErrorView view,
            PreHomeTryAgainNetworkErrorModel model,
            ILogger<PreHomeTryAgainNetworkErrorPresenter> logger)
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