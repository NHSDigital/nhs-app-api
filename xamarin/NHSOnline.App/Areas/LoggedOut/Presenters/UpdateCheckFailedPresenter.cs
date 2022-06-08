using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class UpdateCheckFailedPresenter
    {
        private readonly IUpdateCheckFailedView _view;
        private readonly IBrowser _browser;
        private readonly INhsExternalServicesConfiguration _nhsExternalServices;
        private readonly ILogger<UpdateCheckFailedPresenter> _logger;
        readonly INavigationService _navigationService;

        public UpdateCheckFailedPresenter(
            IUpdateCheckFailedView view,
            IBrowser browser,
            INhsExternalServicesConfiguration nhsExternalServices,
            ILogger<UpdateCheckFailedPresenter> logger, INavigationService navigationService)
        {
            _view = view;
            _browser = browser;
            _nhsExternalServices = nhsExternalServices;
            _logger = logger;
            _navigationService = navigationService;

            view.AppNavigation
                .RegisterHandler(OneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(BackToLoginRequested, (view, handler) => view.BackToLoginRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler);
        }

        private async Task OneOneOneRequested()
        {
            _logger.LogInformation("OneOneOne requested");
            await _browser
                .OpenBrowserOverlay(_nhsExternalServices.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async Task BackToLoginRequested()
        {
            _logger.LogInformation("Back to login requested");
            await _navigationService.Pop().PreserveThreadContext();
        }

        private async Task BackRequested()
        {
            _logger.LogInformation("Back requested");
            await _navigationService.Pop().PreserveThreadContext();
        }
    }
}