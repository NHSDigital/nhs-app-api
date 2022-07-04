using System.Threading.Tasks;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorInternalServerErrorPresenter
    {
        private readonly ICreateSessionErrorInternalServerErrorView _internalServerErrorView;
        private readonly CreateSessionErrorInternalServerErrorModel _internalServerErrorModel;
        private readonly IBrowser _browser;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorInternalServerErrorPresenter(
            ICreateSessionErrorInternalServerErrorView internalServerErrorView,
            CreateSessionErrorInternalServerErrorModel internalServerErrorModel,
            IBrowser browser,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _internalServerErrorView = internalServerErrorView;
            _internalServerErrorModel = internalServerErrorModel;
            _browser = browser;
            _externalServicesConfiguration = externalServicesConfiguration;

            _internalServerErrorView.ServiceDeskReference = internalServerErrorModel.ServiceDeskReference;

            _internalServerErrorView.AppNavigation
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler)
                .RegisterHandler(ViewOnDigitalCovidPassRequested, (view, handler) => view.DigitalCovidCertRequested = handler);
        }

        private async Task ViewOnOneOneOneRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnDigitalCovidPassRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.DigitalCovidPassUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnBackHomeRequested()
        {
            await _internalServerErrorView.AppNavigation
                .PopToRoot()
                .PreserveThreadContext();
        }
    }
}