using System.Threading.Tasks;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorBadResponseFromUpstreamSystemPresenter
    {
        private readonly ICreateSessionErrorBadResponseFromUpstreamSystemView _view;
        private readonly CreateSessionErrorBadResponseFromUpstreamSystemModel _model;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorBadResponseFromUpstreamSystemPresenter(
            ICreateSessionErrorBadResponseFromUpstreamSystemView view,
            CreateSessionErrorBadResponseFromUpstreamSystemModel model,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.ServiceDeskReference = model.ServiceDeskReference;

            _view.AppNavigation
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
            var contactUsUri = _externalServicesConfiguration.NhsUkContactUsUrlWithErrorCode(_model.ServiceDeskReference);
            await _browserOverlay
                .OpenBrowserOverlay(contactUsUri)
                .PreserveThreadContext();
        }

        private async Task ViewOnBackHomeRequested()
        {
            await _view.AppNavigation
                .PopToRoot()
                .PreserveThreadContext();
        }
    }
}