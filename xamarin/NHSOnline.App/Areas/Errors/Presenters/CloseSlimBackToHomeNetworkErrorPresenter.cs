using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class CloseSlimBackToHomeNetworkErrorPresenter
    {
        private readonly ICloseSlimBackToHomeNetworkErrorView _view;
        private readonly CloseSlimBackToHomeNetworkErrorModel _model;
        private readonly ILogger<CloseSlimBackToHomeNetworkErrorPresenter> _logger;

        public CloseSlimBackToHomeNetworkErrorPresenter(
            ICloseSlimBackToHomeNetworkErrorView view,
            CloseSlimBackToHomeNetworkErrorModel model,
            ILogger<CloseSlimBackToHomeNetworkErrorPresenter> logger)
        {
            _view = view;
            _model = model;
            _logger = logger;

            view.AppNavigation
                .RegisterHandler(CloseRequested, (view, handler) => view.CloseRequested = handler)
                .RegisterHandler(BackToHomeRequested, (view, handler) => view.BackToHomeRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler);
        }

        private Task CloseRequested()
        {
            _logger.LogInformation("Close requested");
            return _model.BackToHomeAction.Invoke();
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