using System.Threading.Tasks;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorOdsCodeNotFoundPresenter
    {
        private readonly ICreateSessionErrorOdsCodeNotFoundView _view;
        private readonly CreateSessionErrorOdsCodeNotFoundModel _model;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorOdsCodeNotFoundPresenter(
            ICreateSessionErrorOdsCodeNotFoundView view,
            CreateSessionErrorOdsCodeNotFoundModel model,
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
                .RegisterHandler(ViewCovidStatusServiceRequested, (view, handler) => view.CovidStatusServiceRequested = handler)
                .RegisterHandler(ViewCovidPassRequested, (view, handler) => view.CovidPassRequested = handler);
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

        private async Task ViewCovidStatusServiceRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.CovidStatusService)
                .PreserveThreadContext();
        }

        private async Task ViewCovidPassRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_externalServicesConfiguration.CovidPassUrl)
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