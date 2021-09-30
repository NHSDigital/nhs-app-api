using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class UpdateCheckFailedPresenter
    {
        private readonly IUpdateCheckFailedView _view;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _nhsExternalServices;
        private readonly ILogger<UpdateCheckFailedPresenter> _logger;

        public UpdateCheckFailedPresenter(
            IUpdateCheckFailedView view,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration nhsExternalServices,
            ILogger<UpdateCheckFailedPresenter> logger)
        {
            _view = view;
            _browserOverlay = browserOverlay;
            _nhsExternalServices = nhsExternalServices;
            _logger = logger;

            view.AppNavigation
                .RegisterHandler(OneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(BackToLoginRequested, (view, handler) => view.BackToLoginRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler);
        }

        private async Task OneOneOneRequested()
        {
            _logger.LogInformation("OneOneOne requested");
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServices.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async Task BackToLoginRequested()
        {
            _logger.LogInformation("Back to login requested");
            await _view.AppNavigation.Pop().PreserveThreadContext();
        }

        private async Task BackRequested()
        {
            _logger.LogInformation("Back requested");
            await _view.AppNavigation.Pop().PreserveThreadContext();
        }
    }
}