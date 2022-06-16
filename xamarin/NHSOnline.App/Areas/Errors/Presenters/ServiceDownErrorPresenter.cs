using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class ServiceDownErrorPresenter
    {
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IBrowser _browser;
        private readonly ILogger<ServiceDownErrorPresenter> _logger;
        private readonly IServiceDownErrorView _view;

        public ServiceDownErrorPresenter(
            IServiceDownErrorView view,
            ServiceDownErrorModel model,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IBrowser browser,
            ILogger<ServiceDownErrorPresenter> logger)
        {
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _browser = browser;
            _logger = logger;

            _view = view;
            view.SetNavigationFooterItem(model.SelectedFooterItem);

            view.AppNavigation
                .RegisterHandler(OneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(model.NavigationHandler.HomeRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler(model.NavigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(HelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler(model.NavigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler)
                .RegisterHandler(model.NavigationHandler.AdviceRequested, (view, handler) => view.AdviceRequested = handler)
                .RegisterHandler(model.NavigationHandler.AppointmentsRequested, (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(model.NavigationHandler.PrescriptionsRequested, (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(model.NavigationHandler.YourHealthRequested, (view, handler) => view.YourHealthRequested = handler)
                .RegisterHandler(model.NavigationHandler.MessagesRequested, (view, handler) => view.MessagesRequested = handler);
        }

        private async Task HelpRequested()
        {
            _logger.LogInformation("Help requested");

            await _browser
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }
        private async Task OneOneOneRequested()
        {
            _logger.LogInformation("OneOneOne requested");

            await _browser
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }
    }
}