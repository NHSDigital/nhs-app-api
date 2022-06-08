using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class CloseSlimBackToHomeNetworkErrorPresenter
    {
        private readonly ICloseSlimBackToHomeNetworkErrorView _view;
        private readonly CloseSlimBackToHomeNetworkErrorModel _model;
        private readonly IBrowser _browser;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;
        private readonly ILogger<CloseSlimBackToHomeNetworkErrorPresenter> _logger;

        public CloseSlimBackToHomeNetworkErrorPresenter(
            ICloseSlimBackToHomeNetworkErrorView view,
            CloseSlimBackToHomeNetworkErrorModel model,
            IBrowser browser,
            INhsExternalServicesConfiguration externalServicesConfiguration,
            ILogger<CloseSlimBackToHomeNetworkErrorPresenter> logger)
        {
            _view = view;
            _model = model;
            _browser = browser;
            _externalServicesConfiguration = externalServicesConfiguration;
            _logger = logger;

            view.AppNavigation
                .RegisterHandler(CloseRequested, (view, handler) => view.CloseRequested = handler)
                .RegisterHandler(OneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(BackToHomeRequested, (view, handler) => view.BackToHomeRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler);
        }

        private Task CloseRequested()
        {
            _logger.LogInformation("Close requested");
            return _model.BackToHomeAction.Invoke();
        }

        private async Task OneOneOneRequested()
        {
            _logger.LogInformation("OneOneOne requested");

            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private Task BackToHomeRequested()
        {
            _logger.LogInformation("Back To Home requested");
            return _model.BackToHomeAction.Invoke();
        }

        private Task BackRequested()
        {
            _logger.LogInformation("Back requested");
            return _model.BackToHomeAction.Invoke();
        }
    }
}