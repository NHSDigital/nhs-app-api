using System.Threading.Tasks;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginTermsAndConditionsDeclinedPresenter
    {
        private readonly INhsLoginTermsAndConditionsDeclinedView _view;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public NhsLoginTermsAndConditionsDeclinedPresenter(
            INhsLoginTermsAndConditionsDeclinedView view,
            IBrowserOverlay browserOverlay,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _browserOverlay = browserOverlay;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.AppNavigation
                .RegisterHandler(ViewOnBackToHomeRequested, (view, handler) => view.BackToHomeRequested = handler)
                .RegisterHandler(ViewOnOneOneOneRequested, (view, handler) => view.OneOneOneRequested = handler);
        }

        private async Task ViewOnBackToHomeRequested()
        {
            await _view.AppNavigation
                .PopToRoot()
                .PreserveThreadContext();
        }

        private async Task ViewOnOneOneOneRequested()
        {
            var oneOneOneUri = _externalServicesConfiguration.OneOneOneUrl;
            await _browserOverlay
                .OpenBrowserOverlay(oneOneOneUri)
                .PreserveThreadContext();
        }
    }
}