using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginErrorPresenter
    {
        private readonly INhsLoginErrorView _view;
        private readonly NhsLoginErrorModel _model;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly IPageFactory _pageFactory;
        private readonly ILogger<NhsLoginErrorPresenter> _logger;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public NhsLoginErrorPresenter(
            INhsLoginErrorView view,
            NhsLoginErrorModel model,
            IBrowserOverlay browserOverlay,
            IPageFactory pageFactory,
            ILogger<NhsLoginErrorPresenter> logger,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _browserOverlay = browserOverlay;
            _pageFactory = pageFactory;
            _logger = logger;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.ServiceDeskReference = model.ServiceDeskReference;
            _view.AccessibleServiceDeskReference = model.AccessibleServiceDeskReference;

            _view.AppNavigation
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler)
                .RegisterHandler(ViewOnContactUsRequested, (view, handler) => view.ContactUsRequested = handler)
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(ViewOnCloseRequested, (view, handler) => view.CloseRequested = handler);
        }

        private Task ViewOnBackHomeRequested()
        {
            _logger.LogInformation("Back Home Requested");

            return NavigateToLoggedOutHomeScreen();
        }

        private async Task ViewOnOneOneOneRequested()
        {
            _logger.LogInformation("OneOneOne Requested");

            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnContactUsRequested()
        {
            _logger.LogInformation("Contact Us Requested");

            var contactUsUri = _externalServicesConfiguration.NhsUkContactUsUrlWithErrorCode(_model.ServiceDeskReference);
            await _browserOverlay
                .OpenBrowserOverlay(contactUsUri)
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