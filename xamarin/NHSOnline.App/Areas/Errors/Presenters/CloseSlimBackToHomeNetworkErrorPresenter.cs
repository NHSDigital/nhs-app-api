using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class CloseSlimBackToHomeNetworkErrorPresenter
    {
        private readonly ICloseSlimBackToHomeNetworkErrorView _view;
        private readonly ILogger<CloseSlimBackToHomeNetworkErrorPresenter> _logger;

        public CloseSlimBackToHomeNetworkErrorPresenter(
            ICloseSlimBackToHomeNetworkErrorView view,
            ILogger<CloseSlimBackToHomeNetworkErrorPresenter> logger)
        {
            _view = view;
            _logger = logger;

            view.AppNavigation
                .RegisterHandler(BackToHomeRequested, (view, handler) => view.BackToHomeRequested = handler)
                .RegisterHandler(BackToHomeRequested, (view, handler) => view.BackRequested = handler);
        }

        private Task BackToHomeRequested()
        {
            _logger.LogInformation("Back To Home requested");
            return _view.AppNavigation.PopToRoot();
        }
    }
}