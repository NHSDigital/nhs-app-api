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
        private readonly IBrowser _browser;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorOdsCodeNotFoundPresenter(
            ICreateSessionErrorOdsCodeNotFoundView view,
            CreateSessionErrorOdsCodeNotFoundModel model,
            IBrowser browser,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _browser = browser;
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
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.MyHealthOnlineUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnOneOneOneWalesRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneWalesUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnOneOneOneRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async Task ViewCovidStatusServiceRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.CovidStatusService)
                .PreserveThreadContext();
        }

        private async Task ViewCovidPassRequested()
        {
            await _browser
                .OpenBrowserOverlay(_externalServicesConfiguration.CovidPassUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnContactUsRequested()
        {
            var contactUsUri = _externalServicesConfiguration.NhsUkContactUsUrlWithErrorCode(_model.ServiceDeskReference);
            await _browser
                .OpenBrowserOverlay(contactUsUri)
                .PreserveThreadContext();
        }
    }
}