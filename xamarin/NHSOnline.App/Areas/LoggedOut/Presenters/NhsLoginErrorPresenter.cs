using System.Threading.Tasks;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginErrorPresenter
    {
        private readonly INhsLoginErrorView _view;
        private readonly NhsLoginErrorModel _model;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public NhsLoginErrorPresenter(
            INhsLoginErrorView view,
            NhsLoginErrorModel model,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.ServiceDeskReference = model.ServiceDeskReference;

            _view.AppNavigation
                .RegisterHandler(ViewOnBackHomeRequested, (view, handler) => view.BackHomeRequested = handler)
                .RegisterHandler(ViewOnContactUsRequested, (view, handler) => view.ContactUsRequested = handler);
        }

        private async Task ViewOnBackHomeRequested()
        {
            await _view.AppNavigation
                .PopToRoot()
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