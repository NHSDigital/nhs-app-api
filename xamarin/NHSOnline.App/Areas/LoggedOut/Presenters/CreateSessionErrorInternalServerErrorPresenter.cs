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
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorInternalServerErrorPresenter(
            ICreateSessionErrorInternalServerErrorView internalServerErrorView,
            CreateSessionErrorInternalServerErrorModel internalServerErrorModel,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _internalServerErrorView = internalServerErrorView;
            _internalServerErrorModel = internalServerErrorModel;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;

            _internalServerErrorView.ServiceDeskReference = internalServerErrorModel.ServiceDeskReference;

            _internalServerErrorView.AppNavigation
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(ViewOnContactUsRequested, (view, handler) => view.ContactUsRequested = handler)
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler);
        }

        private async Task ViewOnOneOneOneRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnContactUsRequested()
        {
            var contactUsUri = _externalServicesConfiguration.NhsUkContactUsUrlWithErrorCode(_internalServerErrorModel.ServiceDeskReference);
            await _browserOverlay
                .OpenBrowserOverlay(contactUsUri)
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