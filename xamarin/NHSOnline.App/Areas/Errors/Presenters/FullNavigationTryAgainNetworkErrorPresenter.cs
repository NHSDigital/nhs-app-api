using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class FullNavigationTryAgainNetworkErrorPresenter
    {
        private readonly IFullNavigationTryAgainNetworkErrorView _view;
        private readonly FullNavigationTryAgainNetworkErrorModel _model;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly ILogger<FullNavigationTryAgainNetworkErrorPresenter> _logger;

        public FullNavigationTryAgainNetworkErrorPresenter(
            IFullNavigationTryAgainNetworkErrorView view,
            FullNavigationTryAgainNetworkErrorModel model,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IBrowserOverlay browserOverlay,
            ILogger<FullNavigationTryAgainNetworkErrorPresenter> logger)
        {
            _view = view;
            _model = model;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _browserOverlay = browserOverlay;
            _logger = logger;

            view.SetNavigationFooterItem(model.SelectedFooterItem);

            view.AppNavigation
                .RegisterHandler(TryAgainRequested, (view, handler) => view.TryAgainRequested = handler)
                .RegisterHandler(TryAgainRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler(model.NavigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(HelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler(model.NavigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler)
                .RegisterHandler(model.NavigationHandler.AdviceRequested, (view, handler) => view.AdviceRequested = handler)
                .RegisterHandler(model.NavigationHandler.AppointmentsRequested, (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(model.NavigationHandler.PrescriptionsRequested, (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(model.NavigationHandler.YourHealthRequested, (view, handler) => view.YourHealthRequested = handler)
                .RegisterHandler(model.NavigationHandler.MessagesRequested, (view, handler) => view.MessagesRequested = handler);
        }

        private Task HelpRequested()
        {
            _logger.LogInformation("Help requested");
            return _browserOverlay.OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkBaseHelpUrl);
        }

        private async Task TryAgainRequested()
        {
            _logger.LogInformation("Try Again requested");

            await _view.AppNavigation.Pop().PreserveThreadContext();
            _model.RetryAction.Invoke();
        }
    }
}