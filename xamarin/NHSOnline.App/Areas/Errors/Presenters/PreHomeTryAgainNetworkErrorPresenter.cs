using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class PreHomeTryAgainNetworkErrorPresenter
    {
        private readonly IPreHomeTryAgainNetworkErrorView _view;
        private readonly PreHomeTryAgainNetworkErrorModel _model;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;
        private readonly ILogger<PreHomeTryAgainNetworkErrorPresenter> _logger;

        public PreHomeTryAgainNetworkErrorPresenter(
            IPreHomeTryAgainNetworkErrorView view,
            PreHomeTryAgainNetworkErrorModel model,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration,
            ILogger<PreHomeTryAgainNetworkErrorPresenter> logger)
        {
            _view = view;
            _model = model;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;
            _logger = logger;

            view.AppNavigation
                .RegisterHandler(OneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(TryAgainRequested, (view, handler) => view.TryAgainRequested = handler)
                .RegisterHandler(TryAgainRequested, (view, handler) => view.BackRequested = handler);
        }

        private async Task OneOneOneRequested()
        {
            _logger.LogInformation("OneOneOne requested");

            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async Task TryAgainRequested()
        {
            _logger.LogInformation("Try Again requested");

            await _view.AppNavigation.Pop().PreserveThreadContext();
            _model.RetryAction.Invoke();
        }
    }
}