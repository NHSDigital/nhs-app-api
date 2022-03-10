using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.Errors.Presenters
{
    internal class FullNavigationTryAgainFileDownloadErrorPresenter
    {
        private readonly IFullNavigationTryAgainFileDownloadErrorView _view;
        private readonly FullNavigationTryAgainFileDownloadErrorModel _model;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly ILogger<FullNavigationTryAgainFileDownloadErrorPresenter> _logger;

        public FullNavigationTryAgainFileDownloadErrorPresenter(
            IFullNavigationTryAgainFileDownloadErrorView view,
            FullNavigationTryAgainFileDownloadErrorModel model,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IBrowserOverlay browserOverlay,
            ILogger<FullNavigationTryAgainFileDownloadErrorPresenter> logger)
        {
            _view = view;
            _model = model;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _browserOverlay = browserOverlay;
            _logger = logger;

            view.SetNavigationFooterItem(model.SelectedFooterItem);

            view.AppNavigation
                .RegisterHandler(TryAgainRequested, (view, handler) => view.TryAgainRequested = handler)
                .RegisterHandler(model.NavigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(HelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler(model.NavigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler)
                .RegisterHandler(model.NavigationHandler.AdviceRequested, (view, handler) => view.AdviceRequested = handler)
                .RegisterHandler(model.NavigationHandler.AppointmentsRequested, (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(model.NavigationHandler.PrescriptionsRequested, (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(model.NavigationHandler.YourHealthRequested, (view, handler) => view.YourHealthRequested = handler)
                .RegisterHandler(model.NavigationHandler.MessagesRequested, (view, handler) => view.MessagesRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler);
        }

        private Task HelpRequested()
        {
            _logger.LogInformation("Help requested");
            return _browserOverlay.OpenBrowserOverlay( _model.HelpUrl ?? _nhsExternalServicesConfiguration.NhsUkHealthRecordDownloadHelpUrl);
        }

        private async Task TryAgainRequested()
        {
            _logger.LogInformation("Try Again requested");
            await _view.AppNavigation.Pop().PreserveThreadContext();
        }

        private async Task BackRequested()
        {
            _logger.LogInformation("Back requested");

            await _view.AppNavigation.Pop().PreserveThreadContext();
        }
    }
}