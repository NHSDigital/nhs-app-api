using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginTermsAndConditionsDeclinedPresenter
    {
        private readonly INhsLoginTermsAndConditionsDeclinedView _view;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly IPageFactory _pageFactory;
        private readonly ILogger<NhsLoginTermsAndConditionsDeclinedPresenter> _logger;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public NhsLoginTermsAndConditionsDeclinedPresenter(
            INhsLoginTermsAndConditionsDeclinedView view,
            IBrowserOverlay browserOverlay,
            IPageFactory pageFactory,
            ILogger<NhsLoginTermsAndConditionsDeclinedPresenter> logger,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _browserOverlay = browserOverlay;
            _pageFactory = pageFactory;
            _logger = logger;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.AppNavigation
                .RegisterHandler(ViewOnBackToHomeRequested, (view, handler) => view.BackToHomeRequested = handler)
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(ViewOnCloseRequested, (view, handler) => view.CloseRequested = handler);
        }

        private Task ViewOnBackToHomeRequested()
        {
            _logger.LogInformation("Back Home Requested");

            return NavigateToLoggedOutHomeScreen();
        }

        private async Task ViewOnOneOneOneRequested()
        {
            _logger.LogInformation("OneOneOne Requested");

            var oneOneOneUri = _externalServicesConfiguration.OneOneOneUrl;
            await _browserOverlay
                .OpenBrowserOverlay(oneOneOneUri)
                .PreserveThreadContext();
        }

        private Task ViewOnCloseRequested()
        {
            _logger.LogInformation("Close Requested");

            return NavigateToLoggedOutHomeScreen();
        }

        private Task NavigateToLoggedOutHomeScreen()
        {
            var loggedOutHomeScreenModel = new LoggedOutHomeScreenModel();
            var loggedOutHomeScreenPage = _pageFactory.CreatePageFor(loggedOutHomeScreenModel);

            return _view.AppNavigation
                .PopToNewRoot(loggedOutHomeScreenPage);
        }
    }
}