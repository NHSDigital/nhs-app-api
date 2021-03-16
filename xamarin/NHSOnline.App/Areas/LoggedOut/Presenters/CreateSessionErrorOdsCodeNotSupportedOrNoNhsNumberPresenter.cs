using System.Threading.Tasks;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPresenter
    {
        private readonly ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView _view;
        private readonly CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel _model;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPresenter(
            ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView view,
            CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberModel model,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.ServiceDeskReference = model.ServiceDeskReference;

            _view.AppNavigation
                .RegisterHandler(ViewOnMyHealthOnlineRequested, (view, handler) => view.MyHealthOnlineRequested = handler)
                .RegisterHandler(ViewOnOneOneOneWalesRequested, (view, handler) => view.OneOneOneWalesRequested = handler)
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler)
                .RegisterHandler(ViewOnContactUsRequested, (view, handler) => view.ContactUsRequested = handler);
        }

        private async Task ViewOnMyHealthOnlineRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.MyHealthOnlineUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnOneOneOneWalesRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneWalesUrl)
                .PreserveThreadContext();
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
    }
}