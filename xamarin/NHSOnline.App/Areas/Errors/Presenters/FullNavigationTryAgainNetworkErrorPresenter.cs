using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class FullNavigationTryAgainNetworkErrorPresenter
    {
        private readonly IFullNavigationTryAgainNetworkErrorView _view;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly ILogger<FullNavigationTryAgainNetworkErrorPresenter> _logger;

        private readonly ITryAgainWebview _tryAgainWebview;

        public FullNavigationTryAgainNetworkErrorPresenter(
            IFullNavigationTryAgainNetworkErrorView view,
            FullNavigationTryAgainNetworkErrorModel model,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IBrowserOverlay browserOverlay,
            ILogger<FullNavigationTryAgainNetworkErrorPresenter> logger)
        {
            _view = view;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _browserOverlay = browserOverlay;
            _logger = logger;

            _tryAgainWebview = model.TryAgainWebview;

            view.SetNavigationFooterItem(model.SelectedFooterItem);

            view.AppNavigation
                .RegisterHandler(ViewOnTryAgainRequested, (view, handler) => view.TryAgainRequested = handler)
                .RegisterHandler(ViewOnBackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler(model.NavigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(ViewOnHelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler(model.NavigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler)
                .RegisterHandler(model.NavigationHandler.AdviceRequested, (view, handler) => view.AdviceRequested = handler)
                .RegisterHandler(model.NavigationHandler.AppointmentsRequested, (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(model.NavigationHandler.PrescriptionsRequested, (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(model.NavigationHandler.YourHealthRequested, (view, handler) => view.YourHealthRequested = handler)
                .RegisterHandler(model.NavigationHandler.MessagesRequested, (view, handler) => view.MessagesRequested = handler);
        }

        private Task ViewOnTryAgainRequested()
        {
            _logger.LogInformation("TryAgain Requested");
            return WebViewTryAgain();
        }

        private Task ViewOnBackRequested()
        {
            _logger.LogInformation("Back Requested");
            return WebViewTryAgain();
        }

        private Task ViewOnHelpRequested()
        {
            _logger.LogInformation("Help Requested");
            return _browserOverlay.OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkTechnicalIssuesHelpUrl);
        }

        private async Task WebViewTryAgain()
        {
            await _view.AppNavigation.Pop().PreserveThreadContext();
            _tryAgainWebview.TryAgain();
        }
    }
}